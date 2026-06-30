using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using WrkPlan.CustomerPortal.Application.Interfaces;
using WrkPlan.CustomerPortal.Domain.Entities;
using WrkPlan.CustomerPortal.Infrastructure.Data;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.Infrastructure.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public AuthResponseDto CreateToken(Guid tenantId, Guid userId, string email, string role)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "wrkplan-api";
        var audience = configuration["Jwt:Audience"] ?? "wrkplan-clients";
        var key = configuration["Jwt:Key"] ?? "ChangeThisInProduction_AtLeast32Characters!";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role),
            new("tenant_id", tenantId.ToString()),
            new("tenant_key", tenantId.ToString("N"))
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(12);

        var token = new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: creds);
        var access = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthResponseDto(access, Guid.NewGuid().ToString("N"), role, userId, tenantId);
    }
}

public class TenantProvisioningService(AdminDbContext adminDbContext, ILogger<TenantProvisioningService> logger) : ITenantProvisioningService
{
    public async Task<TenantProvisionResponseDto> ProvisionTenantAsync(TenantProvisionRequestDto request, CancellationToken ct = default)
    {
        var tenant = new TenantRegistry
        {
            Name = request.CompanyName,
            TenantKey = request.TenantKey.ToLowerInvariant(),
            IsActive = false
        };

        adminDbContext.TenantRegistries.Add(tenant);

        var map = new TenantConnectionMap
        {
            TenantRegistryId = tenant.Id,
            DatabaseName = $"nextgen_tenant_{request.TenantKey.ToLowerInvariant()}",
            EncryptedConnectionString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"Server=.;Database=nextgen_tenant_{request.TenantKey};Trusted_Connection=True;TrustServerCertificate=True"))
        };
        adminDbContext.TenantConnectionMaps.Add(map);

        var customer = new CustomerProfile
        {
            TenantId = tenant.Id,
            CompanyName = request.CompanyName,
            City = "Unspecified",
            Country = "Unspecified",
            IsPortalActive = false
        };
        adminDbContext.CustomerProfiles.Add(customer);

        var initialPassword = "Welcome@123";
        adminDbContext.Users.Add(new AppUser
        {
            TenantId = tenant.Id,
            Email = request.AdminEmail,
            DisplayName = "Customer Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(initialPassword)
        });

        await adminDbContext.SaveChangesAsync(ct);

        logger.LogInformation("Provisioned tenant {TenantKey} mapped to DB {DatabaseName}", tenant.TenantKey, map.DatabaseName);
        return new TenantProvisionResponseDto(tenant.Id, map.DatabaseName, request.AdminEmail, initialPassword);
    }

    public async Task ActivateTenantAfterFirstPaymentAsync(Guid tenantId, CancellationToken ct = default)
    {
        var tenant = await adminDbContext.TenantRegistries.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        if (tenant is null)
        {
            throw new InvalidOperationException("Tenant does not exist.");
        }

        tenant.IsActive = true;
        tenant.ActivatedUtc = DateTime.UtcNow;

        var profile = await adminDbContext.CustomerProfiles.FirstOrDefaultAsync(c => c.TenantId == tenantId, ct);
        if (profile is not null)
        {
            profile.IsPortalActive = true;
        }

        await adminDbContext.SaveChangesAsync(ct);
    }
}

public class ThemePreferenceService(AdminDbContext dbContext) : IThemePreferenceService
{
    public async Task<ThemePreferenceDto> GetAsync(Guid customerProfileId, CancellationToken ct = default)
    {
        var pref = await dbContext.CustomerThemePreferences.FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId, ct);
        return pref is null ? new ThemePreferenceDto(false, 247, 249, 252) : new ThemePreferenceDto(pref.IsDarkMode, pref.BackgroundR, pref.BackgroundG, pref.BackgroundB);
    }

    public async Task<ThemePreferenceDto> SaveAsync(Guid customerProfileId, ThemePreferenceDto request, CancellationToken ct = default)
    {
        var pref = await dbContext.CustomerThemePreferences.FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId, ct);
        if (pref is null)
        {
            pref = new CustomerThemePreference { CustomerProfileId = customerProfileId };
            dbContext.CustomerThemePreferences.Add(pref);
        }

        pref.IsDarkMode = request.IsDarkMode;
        pref.BackgroundR = Math.Clamp(request.BackgroundR, 0, 255);
        pref.BackgroundG = Math.Clamp(request.BackgroundG, 0, 255);
        pref.BackgroundB = Math.Clamp(request.BackgroundB, 0, 255);
        pref.UpdatedUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);
        return new ThemePreferenceDto(pref.IsDarkMode, pref.BackgroundR, pref.BackgroundG, pref.BackgroundB);
    }

    public async Task ResetAsync(Guid customerProfileId, CancellationToken ct = default)
    {
        var pref = await dbContext.CustomerThemePreferences.FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId, ct);
        if (pref is null)
        {
            return;
        }

        dbContext.CustomerThemePreferences.Remove(pref);
        await dbContext.SaveChangesAsync(ct);
    }
}

public class DashboardService(AdminDbContext dbContext) : IDashboardService
{
    public async Task<DashboardResponseDto> GetCustomerHubDashboardAsync(CancellationToken ct = default)
    {
        var activeCustomers = await dbContext.CustomerProfiles.CountAsync(c => c.IsPortalActive, ct);
        var upcomingRenewals = await dbContext.Subscriptions.CountAsync(s => s.RenewalDateUtc <= DateTime.UtcNow.AddDays(30), ct);
        var overdueInvoices = await dbContext.Invoices.CountAsync(i => i.Status == Domain.Enums.RecordStatus.Overdue, ct);

        return new DashboardResponseDto(
            new List<DashboardCardDto>
            {
                new("Active Customers", activeCustomers.ToString(), "ok", "Across all tenants"),
                new("Upcoming Renewals", upcomingRenewals.ToString(), "warning", "Within 30 days"),
                new("Overdue Invoices", overdueInvoices.ToString(), "overdue", "Requires follow-up")
            });
    }

    public async Task<DashboardResponseDto> GetCustomerPortalDashboardAsync(Guid customerProfileId, CancellationToken ct = default)
    {
        var openTickets = await dbContext.SupportTickets.CountAsync(t => t.CustomerProfileId == customerProfileId, ct);
        var nextRenewal = await dbContext.Subscriptions.Where(s => s.CustomerProfileId == customerProfileId).Select(s => s.RenewalDateUtc).FirstOrDefaultAsync(ct);
        var pendingInvoice = await dbContext.Invoices.Where(i => i.CustomerProfileId == customerProfileId).OrderByDescending(i => i.CreatedUtc).FirstOrDefaultAsync(ct);

        return new DashboardResponseDto(
            new List<DashboardCardDto>
            {
                new("Open Support Tickets", openTickets.ToString(), "ok"),
                new("Next Renewal", nextRenewal == default ? "N/A" : nextRenewal.ToString("yyyy-MM-dd"), nextRenewal <= DateTime.UtcNow.AddDays(5) ? "warning" : "ok"),
                new("Invoice Status", pendingInvoice?.Status.ToString() ?? "N/A", pendingInvoice?.Status == Domain.Enums.RecordStatus.Overdue ? "overdue" : "ok")
            });
    }
}

public class StripePaymentService(
    IConfiguration configuration,
    AdminDbContext dbContext,
    ITenantProvisioningService tenantProvisioningService,
    ILogger<StripePaymentService> logger) : IPaymentService
{
    public async Task<string> CreateCheckoutSessionAsync(Guid customerProfileId, decimal amount, bool isSubscription, string cycle, CancellationToken ct = default)
    {
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        var options = new SessionCreateOptions
        {
            Mode = isSubscription ? "subscription" : "payment",
            SuccessUrl = "https://localhost:7001/payment/success",
            CancelUrl = "https://localhost:7001/payment/cancel",
            Metadata = new Dictionary<string, string>
            {
                ["customerProfileId"] = customerProfileId.ToString()
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(amount * 100m),
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = $"WrkPlan {cycle} plan" },
                        Recurring = isSubscription ? new SessionLineItemPriceDataRecurringOptions
                        {
                            Interval = cycle.ToLowerInvariant() switch
                            {
                                "monthly" => "month",
                                "quarterly" => "month",
                                _ => "year"
                            },
                            IntervalCount = cycle.Equals("quarterly", StringComparison.OrdinalIgnoreCase) ? 3 : 1
                        } : null
                    }
                }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);
        return session.Url ?? string.Empty;
    }

    public async Task ProcessWebhookAsync(string json, string signatureHeader, CancellationToken ct = default)
    {
        var secret = configuration["Stripe:WebhookSecret"] ?? string.Empty;
        EventUtility.ValidateSignature(json, signatureHeader, secret);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var idempotencyKey = root.GetProperty("id").GetString() ?? Guid.NewGuid().ToString("N");
        var eventType = root.GetProperty("type").GetString() ?? "unknown";

        var existing = await dbContext.PaymentEvents.FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, ct);
        if (existing is not null)
        {
            logger.LogInformation("Duplicate webhook {EventId} ignored", idempotencyKey);
            return;
        }

        var customerProfileId = Guid.Empty;
        if (root.TryGetProperty("data", out var data) &&
            data.TryGetProperty("object", out var obj) &&
            obj.TryGetProperty("metadata", out var metadata) &&
            metadata.TryGetProperty("customerProfileId", out var customerProfileIdElement))
        {
            var idRaw = customerProfileIdElement.GetString();
            _ = Guid.TryParse(idRaw, out customerProfileId);
        }

        dbContext.PaymentEvents.Add(new PaymentEvent
        {
            CustomerProfileId = customerProfileId,
            Provider = "Stripe",
            EventType = eventType,
            IdempotencyKey = idempotencyKey,
            RawPayload = json
        });

        if (eventType is "checkout.session.completed" && customerProfileId != Guid.Empty)
        {
            var profile = await dbContext.CustomerProfiles.FirstOrDefaultAsync(p => p.Id == customerProfileId, ct);
            if (profile is not null)
            {
                await tenantProvisioningService.ActivateTenantAfterFirstPaymentAsync(profile.TenantId, ct);
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}

public class ContractESignWorkflowService(AdminDbContext dbContext, IHostEnvironment hostEnvironment)
{
    public async Task<ServiceResult> AddESignFieldsToContractAsync(Guid contractId, string eSignFieldsJson, CancellationToken ct = default)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null)
        {
            return ServiceResult.Fail("Contract not found");
        }

        contract.ESignFieldsJson = string.IsNullOrWhiteSpace(eSignFieldsJson) ? "[]" : eSignFieldsJson;
        contract.ESignStatus = "Draft";
        contract.UpdatedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SendContractToCustomerAsync(Guid contractId, CancellationToken ct = default)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null)
        {
            return ServiceResult.Fail("Contract not found");
        }

        contract.ESignStatus = "AwaitingSignature";
        contract.SentToCustomerUtc = DateTime.UtcNow;
        contract.Status = "Sent to Customer";
        contract.SentUtc = DateTime.UtcNow;
        contract.UpdatedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SubmitESignEntryAsync(Guid contractId, string fieldId, string fieldLabel, string valueDataUrl, string signedByName, CancellationToken ct = default)
    {
        var contract = await dbContext.Contracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null)
        {
            return ServiceResult.Fail("Contract not found");
        }

        var existing = await dbContext.ContractESignEntries
            .FirstOrDefaultAsync(x => x.ContractId == contractId && x.FieldId == fieldId, ct);

        if (existing is null)
        {
            dbContext.ContractESignEntries.Add(new ContractESignEntry
            {
                ContractId = contractId,
                FieldId = fieldId,
                FieldLabel = fieldLabel,
                ValueBytes = ToStorageBytes(valueDataUrl),
                ValueDataUrl = valueDataUrl,
                SignedByName = signedByName,
                SignedAtUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.FieldLabel = fieldLabel;
            existing.ValueBytes = ToStorageBytes(valueDataUrl);
            existing.ValueDataUrl = valueDataUrl;
            existing.SignedByName = signedByName;
            existing.SignedAtUtc = DateTime.UtcNow;
            existing.UpdatedUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(ct);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<string>> FinalizeAndGeneratePdfAsync(Guid contractId, CancellationToken ct = default)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null)
        {
            return ServiceResult<string>.Fail("Contract not found");
        }

        var fields = ParseFields(contract.ESignFieldsJson);
        var requiredFieldIds = fields.Where(x => x.Required).Select(x => x.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var entries = await dbContext.ContractESignEntries
            .Where(x => x.ContractId == contractId)
            .ToListAsync(ct);

        var signedFieldIds = entries.Select(x => x.FieldId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!requiredFieldIds.All(signedFieldIds.Contains))
        {
            return ServiceResult<string>.Fail("Not all required fields are signed");
        }

        var outputDirectory = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot", "contracts", "signed");
        Directory.CreateDirectory(outputDirectory);

        using var memoryStream = new MemoryStream();
        using (var writer = new PdfWriter(memoryStream))
        using (var pdf = new PdfDocument(writer))
        using (var document = new Document(pdf))
        {
            document.Add(new Paragraph($"{contract.ContractNumber} - {contract.Title}").SetBold().SetFontSize(14));
            document.Add(new Paragraph(" "));

            var bodyText = await ResolveContractBodyTextAsync(contractId, contract.Title, ct);
            foreach (var line in bodyText.Replace("\r", string.Empty).Split('\n'))
            {
                document.Add(new Paragraph(line));
            }

            document.Add(new Paragraph(" "));

            var orderedFields = fields.OrderBy(x => x.Order).ToList();
            foreach (var field in orderedFields)
            {
                var entry = entries.FirstOrDefault(x => x.FieldId.Equals(field.Id, StringComparison.OrdinalIgnoreCase));
                if (entry is null) continue;

                document.Add(new Paragraph(field.Label).SetBold());
                if (field.Type.Equals("signature", StringComparison.OrdinalIgnoreCase))
                {
                    var imageBytes = entry.ValueBytes is { Length: > 0 }
                        ? entry.ValueBytes
                        : DecodeDataUrl(entry.ValueDataUrl);
                    if (imageBytes.Length > 0)
                    {
                        var imageData = ImageDataFactory.Create(imageBytes);
                        var image = new Image(imageData).ScaleToFit(220, 80);
                        document.Add(image);
                    }
                }
                else
                {
                    if (entry.ValueBytes is { Length: > 0 })
                    {
                        document.Add(new Paragraph(Encoding.UTF8.GetString(entry.ValueBytes)));
                    }
                    else
                    {
                        document.Add(new Paragraph(entry.ValueDataUrl));
                    }
                }

                document.Add(new Paragraph($"Signed by {entry.SignedByName} on {entry.SignedAtUtc:dd MMM yyyy HH:mm} UTC").SetFontSize(9));
                document.Add(new Paragraph(" "));
            }

            var footerText = $"This document was digitally signed on {DateTime.UtcNow:dd MMM yyyy HH:mm} UTC";
            var lastPage = pdf.GetPage(pdf.GetNumberOfPages());
            var canvas = new Canvas(new PdfCanvas(lastPage), lastPage.GetPageSize());
            var font = PdfFontFactory.CreateFont();
            canvas.SetFont(font).SetFontSize(9).ShowTextAligned(footerText, 36, 20, TextAlignment.LEFT);
            canvas.Close();
        }

        var finalPath = Path.Combine(outputDirectory, $"{contractId}.pdf");
        await System.IO.File.WriteAllBytesAsync(finalPath, memoryStream.ToArray(), ct);

        var relativePath = $"contracts/signed/{contractId}.pdf";
        contract.SignedPdfPath = relativePath;
        contract.ESignStatus = "Signed";
        contract.Status = "Signed by Customer";
        contract.CustomerSignedUtc = DateTime.UtcNow;
        contract.UpdatedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);

        return ServiceResult<string>.Ok(relativePath);
    }

    public async Task<bool> AreAllRequiredFieldsSignedAsync(Guid contractId, CancellationToken ct = default)
    {
        var contract = await dbContext.Contracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null) return false;

        var fields = ParseFields(contract.ESignFieldsJson);
        var required = fields.Where(x => x.Required).Select(x => x.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (required.Count == 0) return true;

        var signed = await dbContext.ContractESignEntries.AsNoTracking()
            .Where(x => x.ContractId == contractId)
            .Select(x => x.FieldId)
            .ToListAsync(ct);

        return required.All(id => signed.Contains(id, StringComparer.OrdinalIgnoreCase));
    }

    private static List<ESignFieldDto> ParseFields(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<ESignFieldDto>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private async Task<string> ResolveContractBodyTextAsync(Guid contractId, string fallbackTitle, CancellationToken ct)
    {
        var latestDoc = await dbContext.ContractDocuments.AsNoTracking()
            .Where(x => x.ContractId == contractId)
            .OrderByDescending(x => x.IsLatest)
            .ThenByDescending(x => x.CreatedUtc)
            .FirstOrDefaultAsync(ct);

        if (latestDoc is not null && System.IO.File.Exists(latestDoc.BlobPath))
        {
            try
            {
                return await System.IO.File.ReadAllTextAsync(latestDoc.BlobPath, ct);
            }
            catch
            {
                return fallbackTitle;
            }
        }

        return fallbackTitle;
    }

    private static byte[] DecodeDataUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return [];

        var payload = value;
        var comma = value.IndexOf(',');
        if (comma >= 0 && comma < value.Length - 1)
        {
            payload = value[(comma + 1)..];
        }

        try
        {
            return Convert.FromBase64String(payload);
        }
        catch
        {
            return [];
        }
    }

    private static byte[] ToStorageBytes(string valueDataUrl)
    {
        if (string.IsNullOrWhiteSpace(valueDataUrl)) return [];
        if (valueDataUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return DecodeDataUrl(valueDataUrl);
        }

        return Encoding.UTF8.GetBytes(valueDataUrl);
    }
}

public sealed record ServiceResult(bool Success, string? ErrorMessage = null)
{
    public static ServiceResult Ok() => new(true, null);
    public static ServiceResult Fail(string message) => new(false, message);
}

public sealed record ServiceResult<T>(bool Success, T? Data, string? ErrorMessage = null)
{
    public static ServiceResult<T> Ok(T data) => new(true, data, null);
    public static ServiceResult<T> Fail(string message) => new(false, default, message);
}
