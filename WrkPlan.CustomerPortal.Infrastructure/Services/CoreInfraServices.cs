using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
