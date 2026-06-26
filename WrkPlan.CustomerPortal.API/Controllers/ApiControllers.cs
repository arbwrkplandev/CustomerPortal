using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Swashbuckle.AspNetCore.Annotations;
using WrkPlan.CustomerPortal.Application.Interfaces;
using WrkPlan.CustomerPortal.Domain.Entities;
using WrkPlan.CustomerPortal.Domain.Enums;
using WrkPlan.CustomerPortal.Infrastructure.Data;
using WrkPlan.CustomerPortal.Infrastructure.MultiTenancy;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.API.Controllers;
public record LoginChoiceDto(string Portal);
public record CheckoutRequestDto(Guid CustomerProfileId, decimal Amount, bool IsSubscription, string Cycle);

public class AuthController(IJwtTokenService tokenService, AdminDbContext dbContext) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Authenticate user and issue JWT")]
    public async Task<ActionResult<ApiResponseDto<AuthResponseDto>>> Login([FromBody] AuthRequestDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.TenantKey))
        {
            return BadRequest(new ApiResponseDto<AuthResponseDto>(false, null, new ApiErrorDto("invalid_request", "Email, password and tenant key are required.")));
        }

        var tenant = await dbContext.TenantRegistries.AsNoTracking().FirstOrDefaultAsync(x => x.TenantKey == request.TenantKey, ct);
        if (tenant is null)
        {
            return Unauthorized(new ApiResponseDto<AuthResponseDto>(false, null, new ApiErrorDto("invalid_tenant", "Tenant key not found.")));
        }

        var existingUser = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenant.Id && x.Email == request.Email, ct);

        string role;
        Guid userId;
        if (existingUser is not null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash))
            {
                dbContext.LoginAuditLogs.Add(new LoginAuditLog
                {
                    TenantId = tenant.Id,
                    UserId = existingUser.Id,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                    Success = false
                });
                await dbContext.SaveChangesAsync(ct);
                return Unauthorized(new ApiResponseDto<AuthResponseDto>(false, null, new ApiErrorDto("invalid_credentials", "Invalid email or password.")));
            }

            role = request.Email.Contains("admin", StringComparison.OrdinalIgnoreCase)
                ? "WrkPlanAdmin"
                : "CustomerAdmin";
            userId = existingUser.Id;
        }
        else
        {
            role = request.Email.Contains("admin", StringComparison.OrdinalIgnoreCase)
                ? "WrkPlanAdmin"
                : "CustomerAdmin";
            userId = Guid.NewGuid();
        }

        var response = tokenService.CreateToken(tenant.Id, userId, request.Email, role);

        dbContext.LoginAuditLogs.Add(new LoginAuditLog
        {
            TenantId = tenant.Id,
            UserId = userId,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            Success = true
        });
        await dbContext.SaveChangesAsync(ct);

        return Ok(new ApiResponseDto<AuthResponseDto>(true, response));
    }

    [AllowAnonymous]
    [HttpPost("login-choice")]
    public ActionResult<ApiResponseDto<LoginChoiceDto>> LoginChoice([FromBody] LoginChoiceDto request)
        => Ok(new ApiResponseDto<LoginChoiceDto>(true, request));
}

[Authorize(Roles = "WrkPlanAdmin")]
public class TenantProvisioningController(ITenantProvisioningService service) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TenantProvisionResponseDto>>> Provision([FromBody] TenantProvisionRequestDto request, CancellationToken ct)
        => Ok(new ApiResponseDto<TenantProvisionResponseDto>(true, await service.ProvisionTenantAsync(request, ct)));
}

[Authorize(Roles = "WrkPlanAdmin")]
public class CustomerHubController(IDashboardService dashboardService) : BaseApiController
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponseDto<DashboardResponseDto>>> Dashboard(CancellationToken ct)
        => Ok(new ApiResponseDto<DashboardResponseDto>(true, await dashboardService.GetCustomerHubDashboardAsync(ct)));
}

[Authorize(Roles = "WrkPlanAdmin")]
public class AdminTenantsController(AdminDbContext dbContext) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<TenantSummaryDto>>>> List(CancellationToken ct)
    {
        var tenants = await dbContext.TenantRegistries
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedUtc)
            .ToListAsync(ct);

        var profiles = await dbContext.CustomerProfiles
            .AsNoTracking()
            .ToListAsync(ct);

        var subscriptions = await dbContext.Subscriptions
            .AsNoTracking()
            .ToListAsync(ct);

        var summaries = tenants.Select(t =>
        {
            var profile = profiles.FirstOrDefault(p => p.TenantId == t.Id);
            var sub = profile is not null ? subscriptions.FirstOrDefault(s => s.CustomerProfileId == profile.Id) : null;
            return new TenantSummaryDto(t.Id, t.Name, t.TenantKey, t.IsActive, t.ActivatedUtc, profile?.CompanyName, sub?.PlanName, sub?.Price);
        }).ToList();

        return Ok(new ApiResponseDto<List<TenantSummaryDto>>(true, summaries));
    }

    [HttpPatch("{id:guid}/toggle")]
    public async Task<ActionResult<ApiResponseDto<string>>> Toggle(Guid id, CancellationToken ct)
    {
        var tenant = await dbContext.TenantRegistries.FindAsync([id], ct);
        if (tenant is null) return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Tenant not found.")));
        tenant.IsActive = !tenant.IsActive;
        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, tenant.IsActive ? "activated" : "deactivated"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TenantSummaryDto>>> Create(
        [FromBody] CreateTenantAccountRequestDto request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.TenantName) || string.IsNullOrWhiteSpace(request.TenantKey) ||
            string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.AdminEmail) ||
            string.IsNullOrWhiteSpace(request.AdminPassword))
        {
            return BadRequest(new ApiResponseDto<TenantSummaryDto>(false, null,
                new ApiErrorDto("validation", "Tenant name, key, company, admin email, and password are required.")));
        }

        var tenantKey = request.TenantKey.Trim().ToLowerInvariant();
        if (await dbContext.TenantRegistries.AnyAsync(x => x.TenantKey == tenantKey, ct))
        {
            return Conflict(new ApiResponseDto<TenantSummaryDto>(false, null,
                new ApiErrorDto("duplicate_tenant_key", "Tenant key already exists.")));
        }

        var tenant = new TenantRegistry
        {
            Name = request.TenantName.Trim(),
            TenantKey = tenantKey,
            IsActive = request.ActivateNow,
            ActivatedUtc = request.ActivateNow ? DateTime.UtcNow : null
        };
        dbContext.TenantRegistries.Add(tenant);

        var profile = new CustomerProfile
        {
            TenantId = tenant.Id,
            CompanyName = request.CompanyName.Trim(),
            City = "Unspecified",
            Country = "Unspecified",
            IsPortalActive = request.ActivateNow
        };
        dbContext.CustomerProfiles.Add(profile);

        dbContext.Users.Add(new AppUser
        {
            TenantId = tenant.Id,
            Email = request.AdminEmail.Trim(),
            DisplayName = "Customer Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.AdminPassword)
        });

        await dbContext.SaveChangesAsync(ct);

        var dto = new TenantSummaryDto(
            tenant.Id,
            tenant.Name,
            tenant.TenantKey,
            tenant.IsActive,
            tenant.ActivatedUtc,
            profile.CompanyName,
            null,
            null);

        return Ok(new ApiResponseDto<TenantSummaryDto>(true, dto));
    }
}

[Authorize(Roles = "WrkPlanAdmin")]
public class AdminClientsController(AdminDbContext dbContext, ILogger<AdminClientsController> logger) : BaseApiController
{
    [HttpGet("erp-clients")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<ErpClientDto>>>> GetErpClients(
        [FromQuery] PagedRequestDto request, CancellationToken ct)
    {
        try
        {
            var query = dbContext.TB_ERP_CLIENT_DETAIL.AsNoTracking();

            // Fallback for environments where the connection defaults to a different DB.
            if (!await query.AnyAsync(ct))
            {
                query = dbContext.TB_ERP_CLIENT_DETAIL
                    .FromSqlRaw("SELECT [Id], [ClientName], [ClientCode] FROM [nextgen_admin].[dbo].[TB_ERP_CLIENT_DETAIL]")
                    .AsNoTracking();
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(x =>
                    (x.ClientName != null && x.ClientName.Contains(request.Search)) ||
                    (x.ClientCode != null && x.ClientCode.Contains(request.Search)));

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(x => x.ClientName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ErpClientDto(x.Id, x.ClientName, x.ClientCode))
                .ToListAsync(ct);

            var totalPages = total == 0 ? 1 : (int)Math.Ceiling(total / (double)request.PageSize);
            var result = new PagedResultDto<ErpClientDto>(request.PageNumber, request.PageSize, total, totalPages, items);
            return Ok(new ApiResponseDto<PagedResultDto<ErpClientDto>>(true, result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to query TB_ERP_CLIENT_DETAIL");
            return StatusCode(503, new ApiResponseDto<PagedResultDto<ErpClientDto>>(false, null,
                new ApiErrorDto("DB_UNAVAILABLE", "ERP client data is currently unavailable. Verify the database connection.")));
        }
    }
}

[Authorize]
public class SalesQuotesController : BaseApiController
{
    [HttpGet]
    public ActionResult<ApiResponseDto<List<SalesQuoteDto>>> GetQuotes()
        => Ok(new ApiResponseDto<List<SalesQuoteDto>>(true, new List<SalesQuoteDto>()));
}

[Authorize]
public class ContractsController(
    AdminDbContext dbContext,
    TenantContext tenantContext,
    IWebHostEnvironment environment) : BaseApiController
{
    private static readonly HashSet<string> AllowedStatuses =
    [
        "Draft",
        "Sent to Customer",
        "Viewed",
        "Signature Pending",
        "Signed by Customer",
        "Signed by Both",
        "Completed",
        "Rejected/Needs Revision"
    ];

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<ContractSummaryDto>>>> GetContracts(
        [FromQuery] Guid? tenantId,
        CancellationToken ct)
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? string.Empty;

        IQueryable<Contract> query = dbContext.Contracts.AsNoTracking();
        if (string.Equals(role, "CustomerAdmin", StringComparison.OrdinalIgnoreCase))
        {
            var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
            query = query.Where(x => x.CustomerProfileId == customerProfileId);
        }
        else if (tenantId.HasValue)
        {
            var profileIds = await dbContext.CustomerProfiles
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId.Value)
                .Select(x => x.Id)
                .ToListAsync(ct);
            query = query.Where(x => profileIds.Contains(x.CustomerProfileId));
        }

        var contracts = await query.OrderByDescending(x => x.CreatedUtc).ToListAsync(ct);
        var profileMap = await dbContext.CustomerProfiles.AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x.TenantId, ct);
        var tenantIds = profileMap.Values.Distinct().ToList();
        var tenantMap = await dbContext.TenantRegistries.AsNoTracking()
            .Where(x => tenantIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        var response = contracts.Select(x => new ContractSummaryDto(
            x.Id,
            x.ContractNumber,
            x.Title,
            x.Version,
            x.Status,
            x.EffectiveUtc,
            x.ExpiryUtc,
            x.SentUtc,
            x.ViewedUtc,
            x.CustomerSignedUtc,
            x.CompletedUtc,
            profileMap.TryGetValue(x.CustomerProfileId, out var tenantRef) && tenantMap.TryGetValue(tenantRef, out var tenantName)
                ? tenantName
                : null)).ToList();

        return Ok(new ApiResponseDto<List<ContractSummaryDto>>(true, response));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<ContractDetailDto>>> GetDetail(Guid id, CancellationToken ct)
    {
        var contract = await dbContext.Contracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (contract is null)
            return NotFound(new ApiResponseDto<ContractDetailDto>(false, null, new ApiErrorDto("not_found", "Contract not found.")));

        if (User.IsInRole("CustomerAdmin"))
        {
            var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
            if (contract.CustomerProfileId != customerProfileId)
                return Forbid();

            if (contract.ViewedUtc is null)
            {
                var mutable = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == id, ct);
                if (mutable is not null)
                {
                    mutable.ViewedUtc = DateTime.UtcNow;
                    if (mutable.Status == "Sent to Customer")
                    {
                        await AddContractStatusHistoryAsync(mutable, "Viewed", "Contract viewed by customer", null, ct);
                    }
                    await dbContext.SaveChangesAsync(ct);
                    contract = mutable;
                }
            }
        }

        var versions = await dbContext.ContractDocuments
            .AsNoTracking()
            .Where(x => x.ContractId == id)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new ContractVersionDto(x.Id, x.FileName, x.VersionLabel, x.DocumentType, x.IsSigned, x.IsLatest, x.CreatedUtc, x.SizeBytes))
            .ToListAsync(ct);

        var history = await dbContext.ContractStatusHistories
            .AsNoTracking()
            .Where(x => x.ContractId == id)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new ContractStatusHistoryDto(x.Id, x.FromStatus, x.ToStatus, x.Action, x.ActorEmail, x.ActorRole, x.CreatedUtc, x.Notes, x.IpAddress))
            .ToListAsync(ct);

        var detail = new ContractDetailDto(
            contract.Id,
            contract.CustomerProfileId,
            contract.ContractNumber,
            contract.Title,
            ExtractHeader(contract.Title),
            ExtractBody(contract.Title),
            contract.Version,
            contract.Status,
            contract.EffectiveUtc,
            contract.ExpiryUtc,
            contract.SentUtc,
            contract.ViewedUtc,
            contract.CustomerSignedUtc,
            contract.CompletedUtc,
            contract.LastActionBy,
            versions,
            history);

        return Ok(new ApiResponseDto<ContractDetailDto>(true, detail));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpGet("profiles")]
    public async Task<ActionResult<ApiResponseDto<List<ContractCustomerProfileOptionDto>>>> GetCustomerProfiles(CancellationToken ct)
    {
        var profiles = await dbContext.CustomerProfiles.AsNoTracking()
            .OrderBy(x => x.CompanyName)
            .Select(x => new
            {
                x.Id,
                x.TenantId,
                x.CompanyName
            })
            .ToListAsync(ct);

        var profileIds = profiles.Select(x => x.Id).ToList();
        var contacts = await dbContext.Contacts.AsNoTracking()
            .Where(x => profileIds.Contains(x.CustomerProfileId))
            .GroupBy(x => x.CustomerProfileId)
            .Select(g => new { CustomerProfileId = g.Key, Email = g.Select(c => c.Email).FirstOrDefault() })
            .ToDictionaryAsync(x => x.CustomerProfileId, x => x.Email, ct);

        var tenantIds = profiles.Select(x => x.TenantId).Distinct().ToList();
        var tenants = await dbContext.TenantRegistries.AsNoTracking()
            .Where(x => tenantIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        var result = profiles.Select(x =>
        {
            var tenantName = tenants.TryGetValue(x.TenantId, out var t) ? t : "Unknown tenant";
            var contactEmail = contacts.TryGetValue(x.Id, out var email) ? email : null;
            return new ContractCustomerProfileOptionDto(x.Id, x.TenantId, tenantName, x.CompanyName, contactEmail);
        }).ToList();

        return Ok(new ApiResponseDto<List<ContractCustomerProfileOptionDto>>(true, result));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("generated")]
    public async Task<ActionResult<ApiResponseDto<ContractSummaryDto>>> CreateGenerated(
        [FromBody] CreateGeneratedContractDto request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ContractNumber) || string.IsNullOrWhiteSpace(request.BodyJson))
        {
            return BadRequest(new ApiResponseDto<ContractSummaryDto>(false, null,
                new ApiErrorDto("validation", "Contract number and body are required.")));
        }

        var contract = new Contract
        {
            CustomerProfileId = request.CustomerProfileId,
            ContractNumber = request.ContractNumber.Trim(),
            EffectiveUtc = request.EffectiveUtc,
            RenewalUtc = request.ExpiryUtc,
            ExpiryUtc = request.ExpiryUtc,
            Title = string.IsNullOrWhiteSpace(request.Title) ? "WrkPlan Service Agreement" : request.Title.Trim(),
            Version = string.IsNullOrWhiteSpace(request.Version) ? "v1" : request.Version.Trim(),
            Status = NormalizeStatus(request.Status),
            LastActionBy = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name
        };
        dbContext.Contracts.Add(contract);

        var content = BuildGeneratedContractText(contract.Title, request.HeaderText, request.BodyJson, contract.ContractNumber, contract.EffectiveUtc, contract.ExpiryUtc, contract.Version);
        var fileName = $"{contract.ContractNumber}-{contract.Version}.pdf";
        var filePath = WriteContractFile(content, fileName);

        dbContext.ContractDocuments.Add(new ContractDocument
        {
            ContractId = contract.Id,
            FileName = fileName,
            BlobPath = filePath,
            DocumentType = "Generated",
            VersionLabel = contract.Version,
            IsLatest = true,
            ContentType = "application/pdf",
            SizeBytes = Encoding.UTF8.GetByteCount(content)
        });

        await AddContractStatusHistoryAsync(contract, contract.Status, "Generated contract created", "Generated", ct);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new ApiResponseDto<ContractSummaryDto>(true, new ContractSummaryDto(
            contract.Id,
            contract.ContractNumber,
            contract.Title,
            contract.Version,
            contract.Status,
            contract.EffectiveUtc,
            contract.ExpiryUtc,
            contract.SentUtc,
            contract.ViewedUtc,
            contract.CustomerSignedUtc,
            contract.CompletedUtc)));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("manual")]
    public async Task<ActionResult<ApiResponseDto<ContractSummaryDto>>> UploadManual(
        [FromBody] UploadManualContractDto request,
        CancellationToken ct)
    {
        var bytes = DecodeBase64(request.FileBase64);
        if (bytes.Length == 0)
        {
            return BadRequest(new ApiResponseDto<ContractSummaryDto>(false, null,
                new ApiErrorDto("validation", "Manual contract file is required.")));
        }

        var contract = new Contract
        {
            CustomerProfileId = request.CustomerProfileId,
            ContractNumber = request.ContractNumber.Trim(),
            EffectiveUtc = request.EffectiveUtc,
            RenewalUtc = request.ExpiryUtc,
            ExpiryUtc = request.ExpiryUtc,
            Title = "WrkPlan Manual Contract",
            Version = string.IsNullOrWhiteSpace(request.Version) ? "v1" : request.Version,
            Status = "Draft",
            LastActionBy = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name
        };
        dbContext.Contracts.Add(contract);

        var safeName = string.IsNullOrWhiteSpace(request.FileName) ? $"{contract.ContractNumber}-{contract.Version}.pdf" : request.FileName;
        var path = WriteContractBinary(bytes, safeName);

        dbContext.ContractDocuments.Add(new ContractDocument
        {
            ContractId = contract.Id,
            FileName = safeName,
            BlobPath = path,
            DocumentType = "Manual",
            VersionLabel = contract.Version,
            IsLatest = true,
            ContentType = request.ContentType,
            SizeBytes = bytes.LongLength
        });

        await AddContractStatusHistoryAsync(contract, "Draft", "Manual contract uploaded", "Manual upload", ct);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new ApiResponseDto<ContractSummaryDto>(true, new ContractSummaryDto(
            contract.Id,
            contract.ContractNumber,
            contract.Title,
            contract.Version,
            contract.Status,
            contract.EffectiveUtc,
            contract.ExpiryUtc,
            contract.SentUtc,
            contract.ViewedUtc,
            contract.CustomerSignedUtc,
            contract.CompletedUtc)));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("asset")]
    public async Task<ActionResult<ApiResponseDto<string>>> UploadAsset([FromBody] UploadContractAssetDto request, CancellationToken ct)
    {
        var bytes = DecodeBase64(request.FileBase64);
        if (bytes.Length == 0)
        {
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Asset file is required.")));
        }

        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == request.ContractId, ct);
        if (contract is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Contract not found.")));

        var path = WriteContractBinary(bytes, request.FileName);
        dbContext.ContractAssets.Add(new ContractAsset
        {
            ContractId = contract.Id,
            AssetType = request.AssetType,
            FileName = request.FileName,
            BlobPath = path,
            ContentType = request.ContentType
        });
        await AddContractStatusHistoryAsync(contract, contract.Status, $"Asset uploaded: {request.AssetType}", null, ct);
        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "asset_uploaded"));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("status")]
    public async Task<ActionResult<ApiResponseDto<string>>> TransitionStatus([FromBody] ContractStatusTransitionDto request, CancellationToken ct)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == request.ContractId, ct);
        if (contract is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Contract not found.")));

        var target = NormalizeStatus(request.ToStatus);
        if (!AllowedStatuses.Contains(target))
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Invalid contract status.")));

        await AddContractStatusHistoryAsync(contract, target, "Status transition", request.Notes, ct);
        if (target == "Sent to Customer") contract.SentUtc = DateTime.UtcNow;
        if (target == "Completed") contract.CompletedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, target));
    }

    [Authorize(Roles = "CustomerAdmin")]
    [HttpPost("customer/manual-signed")]
    public async Task<ActionResult<ApiResponseDto<string>>> UploadSignedManual([FromBody] UploadSignedContractDto request, CancellationToken ct)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == request.ContractId, ct);
        if (contract is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Contract not found.")));

        var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
        if (contract.CustomerProfileId != customerProfileId)
            return Forbid();

        var bytes = DecodeBase64(request.FileBase64);
        if (bytes.Length == 0)
        {
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Signed file is required.")));
        }

        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? $"{contract.ContractNumber}-{contract.Version}-signed-manual.pdf"
            : request.FileName;

        var path = WriteContractBinary(bytes, fileName);
        dbContext.ContractDocuments.Add(new ContractDocument
        {
            ContractId = contract.Id,
            FileName = fileName,
            BlobPath = path,
            DocumentType = "SignedManual",
            VersionLabel = contract.Version,
            IsLatest = false,
            IsSigned = true,
            ContentType = request.ContentType,
            SizeBytes = bytes.LongLength
        });

        await AddContractStatusHistoryAsync(contract, "Signed by Customer", "Customer uploaded manually signed contract", request.Notes, ct);
        contract.CustomerSignedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "manual_signed_uploaded"));
    }

    [Authorize(Roles = "CustomerAdmin")]
    [HttpPost("customer/apply-signature")]
    public async Task<ActionResult<ApiResponseDto<string>>> ApplyDigitalSignature([FromBody] CustomerApplySignatureDto request, CancellationToken ct)
    {
        var contract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == request.ContractId, ct);
        if (contract is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Contract not found.")));

        var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
        if (contract.CustomerProfileId != customerProfileId)
            return Forbid();

        var signBytes = DecodeBase64(request.SignatureBase64);
        if (signBytes.Length == 0)
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Signature image is required.")));

        var signaturePath = WriteContractBinary(signBytes, request.SignatureFileName);
        dbContext.ContractSignatures.Add(new ContractSignature
        {
            ContractId = contract.Id,
            SignerRole = "Customer",
            SignerEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "customer",
            SignatureBlobPath = signaturePath,
            PlacementXPercent = request.PlacementXPercent,
            PlacementYPercent = request.PlacementYPercent,
            SignedUtc = DateTime.UtcNow
        });

        if (!request.Confirm)
        {
            await AddContractStatusHistoryAsync(contract, "Signature Pending", "Signature preview created", null, ct);
            await dbContext.SaveChangesAsync(ct);
            return Ok(new ApiResponseDto<string>(true, "preview_ready"));
        }

        var generated = BuildGeneratedContractText(
            contract.Title,
            "WrkPlan",
            "This document was signed digitally through the customer portal.",
            contract.ContractNumber,
            contract.EffectiveUtc,
            contract.ExpiryUtc,
            contract.Version + "-signed") +
            $"\n\nSignature placed at X={request.PlacementXPercent}% Y={request.PlacementYPercent}%";

        var signedFileName = $"{contract.ContractNumber}-{contract.Version}-signed-system.pdf";
        var signedPath = WriteContractFile(generated, signedFileName);

        dbContext.ContractDocuments.Add(new ContractDocument
        {
            ContractId = contract.Id,
            FileName = signedFileName,
            BlobPath = signedPath,
            DocumentType = "SignedGenerated",
            VersionLabel = contract.Version,
            IsLatest = false,
            IsSigned = true,
            ContentType = "application/pdf",
            SizeBytes = Encoding.UTF8.GetByteCount(generated)
        });

        contract.CustomerSignedUtc = DateTime.UtcNow;
        await AddContractStatusHistoryAsync(contract, "Signed by Customer", "Customer digitally signed in portal", null, ct);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new ApiResponseDto<string>(true, "signed_pdf_generated"));
    }

    [HttpGet("documents/{documentId:guid}/download")]
    public async Task<IActionResult> DownloadDocument(Guid documentId, CancellationToken ct)
    {
        var doc = await dbContext.ContractDocuments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == documentId, ct);
        if (doc is null || !System.IO.File.Exists(doc.BlobPath))
            return NotFound();

        var contract = await dbContext.Contracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == doc.ContractId, ct);
        if (contract is null)
            return NotFound();

        if (User.IsInRole("CustomerAdmin"))
        {
            var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
            if (contract.CustomerProfileId != customerProfileId)
                return Forbid();
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(doc.BlobPath, ct);
        return File(bytes, doc.ContentType ?? "application/octet-stream", doc.FileName);
    }

    private static string NormalizeStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "Draft";
        return AllowedStatuses.FirstOrDefault(x => x.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase)) ?? "Draft";
    }

    private async Task<Guid> ResolveCustomerProfileIdAsync(CancellationToken ct)
    {
        return await dbContext.CustomerProfiles.AsNoTracking()
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);
    }

    private async Task AddContractStatusHistoryAsync(Contract contract, string toStatus, string action, string? notes, CancellationToken ct)
    {
        var fromStatus = contract.Status;
        contract.Status = toStatus;
        contract.LastActionBy = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
        contract.UpdatedUtc = DateTime.UtcNow;
        dbContext.ContractStatusHistories.Add(new ContractStatusHistory
        {
            ContractId = contract.Id,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Action = action,
            ActorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "system",
            ActorRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "Unknown",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Notes = notes
        });
        await Task.CompletedTask;
    }

    private string ContractRootPath
    {
        get
        {
            var path = Path.Combine(environment.ContentRootPath, "App_Data", "Contracts");
            Directory.CreateDirectory(path);
            return path;
        }
    }

    private string WriteContractFile(string content, string fileName)
    {
        var safe = $"{Guid.NewGuid():N}_{SanitizeFileName(fileName)}";
        var fullPath = Path.Combine(ContractRootPath, safe);
        System.IO.File.WriteAllText(fullPath, content, Encoding.UTF8);
        return fullPath;
    }

    private string WriteContractBinary(byte[] bytes, string fileName)
    {
        var safe = $"{Guid.NewGuid():N}_{SanitizeFileName(fileName)}";
        var fullPath = Path.Combine(ContractRootPath, safe);
        System.IO.File.WriteAllBytes(fullPath, bytes);
        return fullPath;
    }

    private static byte[] DecodeBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return [];
        try { return Convert.FromBase64String(base64); }
        catch { return []; }
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(c, '_');
        return string.IsNullOrWhiteSpace(fileName) ? "contract.bin" : fileName;
    }

    private static string BuildGeneratedContractText(string title, string header, string bodyJson, string contractNumber, DateTime effectiveUtc, DateTime expiryUtc, string version)
    {
        return $"WrkPlan\n{title}\n\n{header}\n\nContract Number: {contractNumber}\nVersion: {version}\nEffective: {effectiveUtc:yyyy-MM-dd}\nExpiry: {expiryUtc:yyyy-MM-dd}\n\n{bodyJson}";
    }

    private static string ExtractHeader(string title)
    {
        return title.StartsWith("HDR:", StringComparison.Ordinal) ? title : "WrkPlan";
    }

    private static string ExtractBody(string title)
    {
        return title.StartsWith("BODY:", StringComparison.Ordinal) ? title : "{}";
    }
}

[Authorize]
public class SubscriptionsController : BaseApiController
{
    [HttpGet]
    public IActionResult Get() => Ok(new ApiResponseDto<string>(true, "Subscriptions endpoint ready"));
}

[Authorize]
public class BillingController(AdminDbContext dbContext, TenantContext tenantContext) : BaseApiController
{
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponseDto<BillingMonitorItemDto>>> Summary(CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .AsNoTracking()
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (customerProfileId == Guid.Empty)
            return NotFound(new ApiResponseDto<BillingMonitorItemDto>(false, null, new ApiErrorDto("profile_not_found", "Customer profile not found.")));

        var profile = await dbContext.CustomerProfiles.AsNoTracking().FirstAsync(x => x.Id == customerProfileId, ct);
        var tenant = await dbContext.TenantRegistries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == profile.TenantId, ct);
        var subscription = await dbContext.Subscriptions.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId, ct);
        var invoice = await dbContext.Invoices.AsNoTracking()
            .Where(x => x.CustomerProfileId == customerProfileId)
            .OrderByDescending(x => x.DueUtc)
            .FirstOrDefaultAsync(ct);

        var item = new BillingMonitorItemDto(
            profile.TenantId,
            profile.Id,
            tenant?.Name ?? "Unknown",
            profile.CompanyName,
            subscription?.PlanName ?? "N/A",
            subscription?.Cycle.ToString() ?? "N/A",
            invoice?.Total ?? 0m,
            invoice?.OutstandingAmount ?? invoice?.Total ?? 0m,
            subscription?.RenewalDateUtc ?? DateTime.UtcNow,
            invoice?.PaymentStatus ?? "Unpaid",
            invoice?.LastPaymentUtc,
            invoice?.LastPaymentMode);

        return Ok(new ApiResponseDto<BillingMonitorItemDto>(true, item));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("monitoring")]
    public async Task<ActionResult<ApiResponseDto<BillingMonitoringResultDto>>> Monitoring([FromBody] BillingMonitoringFilterDto request, CancellationToken ct)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 200 ? 200 : request.PageSize;

        var profiles = await dbContext.CustomerProfiles.AsNoTracking().ToListAsync(ct);
        var tenants = await dbContext.TenantRegistries.AsNoTracking().ToDictionaryAsync(x => x.Id, ct);
        var subscriptions = await dbContext.Subscriptions.AsNoTracking().ToListAsync(ct);
        var invoices = await dbContext.Invoices.AsNoTracking().ToListAsync(ct);

        var rows = profiles.Select(profile =>
        {
            var tenant = tenants.TryGetValue(profile.TenantId, out var tenantRow) ? tenantRow : null;
            var sub = subscriptions.Where(x => x.CustomerProfileId == profile.Id).OrderByDescending(x => x.RenewalDateUtc).FirstOrDefault();
            var invoice = invoices.Where(x => x.CustomerProfileId == profile.Id).OrderByDescending(x => x.DueUtc).FirstOrDefault();
            return new BillingMonitorItemDto(
                profile.TenantId,
                profile.Id,
                tenant?.Name ?? "Unknown",
                profile.CompanyName,
                sub?.PlanName ?? "N/A",
                sub?.Cycle.ToString() ?? "N/A",
                invoice?.Total ?? 0m,
                invoice?.OutstandingAmount ?? invoice?.Total ?? 0m,
                sub?.RenewalDateUtc ?? DateTime.UtcNow,
                invoice?.PaymentStatus ?? "Unpaid",
                invoice?.LastPaymentUtc,
                invoice?.LastPaymentMode);
        });

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            rows = rows.Where(x =>
                x.TenantName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                x.CompanyName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                x.CurrentPlan.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrWhiteSpace(request.PaymentStatus))
            rows = rows.Where(x => string.Equals(x.PaymentStatus, request.PaymentStatus, StringComparison.OrdinalIgnoreCase));
        if (request.RenewalFromUtc.HasValue)
            rows = rows.Where(x => x.NextRenewalDate >= request.RenewalFromUtc.Value);
        if (request.RenewalToUtc.HasValue)
            rows = rows.Where(x => x.NextRenewalDate <= request.RenewalToUtc.Value);

        var sortBy = (request.SortBy ?? "renewal").Trim().ToLowerInvariant();
        var desc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        rows = sortBy switch
        {
            "tenant" => desc ? rows.OrderByDescending(x => x.TenantName) : rows.OrderBy(x => x.TenantName),
            "outstanding" => desc ? rows.OrderByDescending(x => x.Outstanding) : rows.OrderBy(x => x.Outstanding),
            "status" => desc ? rows.OrderByDescending(x => x.PaymentStatus) : rows.OrderBy(x => x.PaymentStatus),
            _ => desc ? rows.OrderByDescending(x => x.NextRenewalDate) : rows.OrderBy(x => x.NextRenewalDate)
        };

        var totalCount = rows.Count();
        var page = rows.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var overdueCount = rows.Count(x => x.Outstanding > 0m && x.NextRenewalDate < DateTime.UtcNow);
        var upcomingCount = rows.Count(x => x.NextRenewalDate >= DateTime.UtcNow && x.NextRenewalDate <= DateTime.UtcNow.AddDays(30));

        var result = new BillingMonitoringResultDto(
            new PagedResultDto<BillingMonitorItemDto>(pageNumber, pageSize, totalCount, totalPages, page),
            overdueCount,
            upcomingCount);

        return Ok(new ApiResponseDto<BillingMonitoringResultDto>(true, result));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("manual-payment")]
    public async Task<ActionResult<ApiResponseDto<string>>> RecordManualPayment([FromBody] ManualPaymentCreateDto request, CancellationToken ct)
    {
        if (request.Amount <= 0)
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Amount must be greater than zero.")));
        if (string.IsNullOrWhiteSpace(request.Mode))
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Payment mode is required.")));

        var profile = await dbContext.CustomerProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerProfileId, ct);
        if (profile is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("profile_not_found", "Customer profile not found.")));

        var entry = new ManualPaymentEntry
        {
            TenantId = profile.TenantId,
            CustomerProfileId = request.CustomerProfileId,
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaidUtc = request.PaidUtc,
            Mode = request.Mode.Trim(),
            ReferenceId = request.ReferenceId.Trim(),
            Notes = request.Notes,
            Status = "Recorded"
        };
        dbContext.ManualPaymentEntries.Add(entry);

        Invoice? invoice = null;
        if (request.InvoiceId.HasValue)
        {
            invoice = await dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == request.InvoiceId.Value, ct);
            if (invoice is not null)
            {
                invoice.OutstandingAmount = Math.Max(0m, (invoice.OutstandingAmount <= 0 ? invoice.Total : invoice.OutstandingAmount) - request.Amount);
                invoice.LastPaymentUtc = request.PaidUtc;
                invoice.LastPaymentMode = request.Mode.Trim();
                invoice.PaymentStatus = invoice.OutstandingAmount <= 0 ? "Paid" : "Partial";
                if (invoice.PaymentStatus == "Paid")
                    invoice.Status = RecordStatus.Inactive;
            }
        }

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId ?? Guid.Empty,
            Amount = request.Amount,
            PaidUtc = request.PaidUtc,
            ProviderRef = request.ReferenceId.Trim(),
            Mode = request.Mode.Trim(),
            Source = "Manual",
            Provider = "Manual",
            Status = "Success",
            Notes = request.Notes
        };
        dbContext.Payments.Add(payment);

        dbContext.PaymentModeHistory.Add(new PaymentModeHistory
        {
            TenantId = profile.TenantId,
            CustomerProfileId = profile.Id,
            InvoiceId = request.InvoiceId,
            ManualPaymentEntryId = entry.Id,
            PaymentId = payment.Id,
            Mode = request.Mode.Trim(),
            Source = "Manual",
            ReferenceId = request.ReferenceId.Trim(),
            OccurredUtc = request.PaidUtc
        });

        dbContext.AuditEvents.Add(new AuditEvent
        {
            TenantId = profile.TenantId,
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            UserRole = "WrkPlanAdmin",
            EventType = "Payment",
            EntityType = "ManualPayment",
            EntityId = entry.Id.ToString(),
            Action = "Manual payment recorded",
            DetailsJson = JsonSerializer.Serialize(new { request.Amount, request.Mode, request.ReferenceId, request.InvoiceId }),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            OccurredUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "manual_payment_recorded"));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpGet("{customerProfileId:guid}/payments")]
    public async Task<ActionResult<ApiResponseDto<List<InvoicePaymentHistoryDto>>>> PaymentHistory(Guid customerProfileId, CancellationToken ct)
    {
        var invoiceIds = await dbContext.Invoices.AsNoTracking()
            .Where(x => x.CustomerProfileId == customerProfileId)
            .Select(x => x.Id)
            .ToListAsync(ct);

        var online = await dbContext.Payments.AsNoTracking()
            .Where(x => invoiceIds.Contains(x.InvoiceId))
            .OrderByDescending(x => x.PaidUtc)
            .Take(150)
            .Select(x => new InvoicePaymentHistoryDto(x.Id, x.Amount, x.PaidUtc, x.Mode, x.Source, x.ProviderRef, x.Notes))
            .ToListAsync(ct);

        return Ok(new ApiResponseDto<List<InvoicePaymentHistoryDto>>(true, online));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpGet("{customerProfileId:guid}/payment-modes")]
    public async Task<ActionResult<ApiResponseDto<List<PaymentModeHistoryDto>>>> PaymentModes(Guid customerProfileId, CancellationToken ct)
    {
        var profile = await dbContext.CustomerProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerProfileId, ct);
        if (profile is null)
            return NotFound(new ApiResponseDto<List<PaymentModeHistoryDto>>(false, null, new ApiErrorDto("profile_not_found", "Customer profile not found.")));

        var items = await dbContext.PaymentModeHistory.AsNoTracking()
            .Where(x => x.CustomerProfileId == customerProfileId)
            .OrderByDescending(x => x.OccurredUtc)
            .Take(200)
            .Select(x => new PaymentModeHistoryDto(x.Mode, x.Source, x.ReferenceId, x.OccurredUtc))
            .ToListAsync(ct);

        return Ok(new ApiResponseDto<List<PaymentModeHistoryDto>>(true, items));
    }
}

[Authorize]
public class PaymentsController(IPaymentService paymentService) : BaseApiController
{
    [HttpPost("checkout")]
    public async Task<ActionResult<ApiResponseDto<string>>> Checkout([FromBody] CheckoutRequestDto request, CancellationToken ct)
    {
        var url = await paymentService.CreateCheckoutSessionAsync(request.CustomerProfileId, request.Amount, request.IsSubscription, request.Cycle, ct);
        return Ok(new ApiResponseDto<string>(true, url));
    }

    [AllowAnonymous]
    [HttpPost("stripe/webhook")]
    public async Task<IActionResult> StripeWebhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync(ct);
        var sig = Request.Headers["Stripe-Signature"].ToString();
        await paymentService.ProcessWebhookAsync(json, sig, ct);
        return Ok();
    }
}

[Authorize(Roles = "WrkPlanAdmin")]
public class RazorpayAdminController(AdminDbContext dbContext, IDataProtectionProvider protectionProvider) : BaseApiController
{
    [HttpGet("settings")]
    public async Task<ActionResult<ApiResponseDto<RazorpaySettingsViewDto>>> GetSettings(CancellationToken ct)
    {
        var settings = await dbContext.RazorpaySettings.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == Guid.Empty, ct);
        if (settings is null)
        {
            return Ok(new ApiResponseDto<RazorpaySettingsViewDto>(true, new RazorpaySettingsViewDto(
                string.Empty,
                true,
                false,
                null,
                false,
                false)));
        }

        var keyId = Decrypt(settings.KeyIdEncrypted);
        var dto = new RazorpaySettingsViewDto(
            Mask(keyId),
            settings.IsTestMode,
            settings.IsActive,
            settings.LastValidatedUtc,
            !string.IsNullOrWhiteSpace(settings.KeySecretEncrypted),
            !string.IsNullOrWhiteSpace(settings.WebhookSecretEncrypted));
        return Ok(new ApiResponseDto<RazorpaySettingsViewDto>(true, dto));
    }

    [HttpPost("settings")]
    public async Task<ActionResult<ApiResponseDto<RazorpaySettingsViewDto>>> SaveSettings([FromBody] RazorpaySettingsUpsertDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.KeyId) || string.IsNullOrWhiteSpace(request.KeySecret) || string.IsNullOrWhiteSpace(request.WebhookSecret))
            return BadRequest(new ApiResponseDto<RazorpaySettingsViewDto>(false, null, new ApiErrorDto("validation", "KeyId, KeySecret and WebhookSecret are required.")));

        var settings = await dbContext.RazorpaySettings.FirstOrDefaultAsync(x => x.TenantId == Guid.Empty, ct);
        if (settings is null)
        {
            settings = new RazorpaySetting { TenantId = Guid.Empty };
            dbContext.RazorpaySettings.Add(settings);
        }

        settings.KeyIdEncrypted = Encrypt(request.KeyId.Trim());
        settings.KeySecretEncrypted = Encrypt(request.KeySecret.Trim());
        settings.WebhookSecretEncrypted = Encrypt(request.WebhookSecret.Trim());
        settings.IsTestMode = request.IsTestMode;
        settings.IsActive = true;
        settings.UpdatedUtc = DateTime.UtcNow;

        if (request.ValidateConnection)
        {
            using var client = new HttpClient();
            var authBytes = Encoding.ASCII.GetBytes($"{request.KeyId}:{request.KeySecret}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            var response = await client.GetAsync("https://api.razorpay.com/v1/payments?count=1", ct);
            if (!response.IsSuccessStatusCode)
                return BadRequest(new ApiResponseDto<RazorpaySettingsViewDto>(false, null, new ApiErrorDto("razorpay_validation_failed", "Unable to validate Razorpay credentials.")));

            settings.LastValidatedUtc = DateTime.UtcNow;
        }

        dbContext.AuditEvents.Add(new AuditEvent
        {
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            UserRole = "WrkPlanAdmin",
            EventType = "Payment",
            EntityType = "RazorpaySettings",
            EntityId = settings.Id.ToString(),
            Action = "Updated",
            DetailsJson = JsonSerializer.Serialize(new { request.IsTestMode, request.ValidateConnection }),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            OccurredUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<RazorpaySettingsViewDto>(true,
            new RazorpaySettingsViewDto(Mask(request.KeyId.Trim()), settings.IsTestMode, settings.IsActive, settings.LastValidatedUtc, true, true)));
    }

    private string Encrypt(string value)
    {
        var protector = protectionProvider.CreateProtector("WrkPlan.Razorpay.Settings");
        return protector.Protect(value);
    }

    private string Decrypt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var protector = protectionProvider.CreateProtector("WrkPlan.Razorpay.Settings");
        try { return protector.Unprotect(value); }
        catch { return string.Empty; }
    }

    private static string Mask(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length <= 4) return "****";
        return $"{value[..2]}****{value[^2..]}";
    }
}

[Authorize(Roles = "CustomerAdmin,WrkPlanAdmin")]
public class RazorpayCheckoutController(AdminDbContext dbContext, TenantContext tenantContext, IDataProtectionProvider protectionProvider) : BaseApiController
{
    [HttpPost("preview")]
    public async Task<ActionResult<ApiResponseDto<PaymentPreviewDto>>> Preview([FromBody] PaymentPreviewRequestDto request, CancellationToken ct)
    {
        var profile = await dbContext.CustomerProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerProfileId, ct);
        if (profile is null)
            return NotFound(new ApiResponseDto<PaymentPreviewDto>(false, null, new ApiErrorDto("profile_not_found", "Customer profile not found.")));

        if (User.IsInRole("CustomerAdmin") && profile.TenantId != tenantContext.TenantId)
            return Forbid();

        var subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(x => x.CustomerProfileId == profile.Id, ct);
        if (subscription is null)
            return NotFound(new ApiResponseDto<PaymentPreviewDto>(false, null, new ApiErrorDto("subscription_not_found", "Subscription not found.")));

        var cycleMonths = CycleMonths(request.Cycle);
        if (cycleMonths <= 0)
            return BadRequest(new ApiResponseDto<PaymentPreviewDto>(false, null, new ApiErrorDto("validation", "Invalid cycle. Use Monthly, Quarterly or Annual.")));

        var baseMonths = subscription.Cycle switch
        {
            SubscriptionCycle.Quarterly => 3,
            SubscriptionCycle.Annual => 12,
            _ => 1
        };
        var monthlyBase = baseMonths <= 0 ? subscription.Price : subscription.Price / baseMonths;
        var planAmount = decimal.Round(monthlyBase * cycleMonths, 2, MidpointRounding.AwayFromZero);
        var taxes = decimal.Round(planAmount * 0.18m, 2, MidpointRounding.AwayFromZero);
        var fees = decimal.Round(planAmount * 0.01m, 2, MidpointRounding.AwayFromZero);
        var total = planAmount + taxes + fees;

        var today = DateTime.UtcNow.Date;
        var startDate = subscription.EndDateUtc.Date >= today ? subscription.EndDateUtc.Date.AddDays(1) : today;
        var endDate = startDate.AddMonths(cycleMonths).AddDays(-1);

        var invoice = new Invoice
        {
            CustomerProfileId = profile.Id,
            InvoiceNumber = $"INV-{profile.CompanyName[..Math.Min(3, profile.CompanyName.Length)].ToUpperInvariant()}-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Total = total,
            OutstandingAmount = total,
            DueUtc = today.AddDays(7),
            Status = RecordStatus.Active,
            PaymentStatus = "Pending"
        };
        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new ApiResponseDto<PaymentPreviewDto>(true, new PaymentPreviewDto(
            profile.Id,
            request.Cycle,
            startDate,
            endDate,
            planAmount,
            taxes,
            fees,
            total,
            invoice.Id,
            invoice.InvoiceNumber,
            "INR")));
    }

    [HttpPost("create-order")]
    public async Task<ActionResult<ApiResponseDto<RazorpayOrderCreateResponseDto>>> CreateOrder([FromBody] RazorpayOrderCreateRequestDto request, CancellationToken ct)
    {
        var invoice = await dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == request.InvoiceId && x.CustomerProfileId == request.CustomerProfileId, ct);
        if (invoice is null)
            return NotFound(new ApiResponseDto<RazorpayOrderCreateResponseDto>(false, null, new ApiErrorDto("invoice_not_found", "Invoice not found.")));

        var setting = await dbContext.RazorpaySettings.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == Guid.Empty && x.IsActive, ct);
        if (setting is null)
            return BadRequest(new ApiResponseDto<RazorpayOrderCreateResponseDto>(false, null, new ApiErrorDto("settings_missing", "Razorpay settings are not configured.")));

        var keyId = Decrypt(setting.KeyIdEncrypted);
        var keySecret = Decrypt(setting.KeySecretEncrypted);
        if (string.IsNullOrWhiteSpace(keyId) || string.IsNullOrWhiteSpace(keySecret))
            return BadRequest(new ApiResponseDto<RazorpayOrderCreateResponseDto>(false, null, new ApiErrorDto("settings_invalid", "Razorpay keys are invalid.")));

        var payload = JsonSerializer.Serialize(new
        {
            amount = (int)Math.Round(invoice.Total * 100m, 0, MidpointRounding.AwayFromZero),
            currency = "INR",
            receipt = invoice.InvoiceNumber,
            payment_capture = 1,
            notes = new { invoiceId = invoice.Id, customerProfileId = request.CustomerProfileId, cycle = request.Cycle }
        });

        using var client = new HttpClient();
        var authBytes = Encoding.ASCII.GetBytes($"{keyId}:{keySecret}");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        var response = await client.PostAsync("https://api.razorpay.com/v1/orders", new StringContent(payload, Encoding.UTF8, "application/json"), ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            return BadRequest(new ApiResponseDto<RazorpayOrderCreateResponseDto>(false, null, new ApiErrorDto("razorpay_order_failed", "Unable to create Razorpay order.")));

        using var doc = JsonDocument.Parse(responseBody);
        var orderId = doc.RootElement.GetProperty("id").GetString() ?? string.Empty;

        var contact = await dbContext.Contacts.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerProfileId == request.CustomerProfileId, ct);
        var profile = await dbContext.CustomerProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerProfileId, ct);

        return Ok(new ApiResponseDto<RazorpayOrderCreateResponseDto>(true, new RazorpayOrderCreateResponseDto(
            keyId,
            orderId,
            invoice.Total,
            "INR",
            $"WrkPlan {request.Cycle} subscription payment",
            profile?.CompanyName ?? "Customer",
            contact?.Email ?? string.Empty,
            invoice.Id,
            invoice.InvoiceNumber)));
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ApiResponseDto<PaymentProcessResultDto>>> Verify([FromBody] RazorpayVerifyRequestDto request, CancellationToken ct)
    {
        var setting = await dbContext.RazorpaySettings.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == Guid.Empty && x.IsActive, ct);
        if (setting is null)
            return BadRequest(new ApiResponseDto<PaymentProcessResultDto>(false, null, new ApiErrorDto("settings_missing", "Razorpay settings not configured.")));

        var webhookSecret = Decrypt(setting.WebhookSecretEncrypted);
        if (string.IsNullOrWhiteSpace(webhookSecret))
            return BadRequest(new ApiResponseDto<PaymentProcessResultDto>(false, null, new ApiErrorDto("settings_invalid", "Webhook secret not configured.")));

        var payload = $"{request.RazorpayOrderId}|{request.RazorpayPaymentId}";
        var expected = ComputeHmacSha256(payload, webhookSecret);
        if (!string.Equals(expected, request.RazorpaySignature, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiResponseDto<PaymentProcessResultDto>(false, null,
                new ApiErrorDto("signature_invalid", "Invalid Razorpay signature.")));
        }

        var invoice = await dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == request.InvoiceId && x.CustomerProfileId == request.CustomerProfileId, ct);
        if (invoice is null)
            return NotFound(new ApiResponseDto<PaymentProcessResultDto>(false, null, new ApiErrorDto("invoice_not_found", "Invoice not found.")));

        if (invoice.PaymentStatus == "Paid")
        {
            return Ok(new ApiResponseDto<PaymentProcessResultDto>(true, new PaymentProcessResultDto(
                true,
                "success",
                "Payment already processed.",
                invoice.Id,
                invoice.InvoiceNumber,
                invoice.LastPaymentUtc)));
        }

        var paidUtc = DateTime.UtcNow;
        var payment = new Payment
        {
            InvoiceId = invoice.Id,
            Amount = invoice.Total,
            PaidUtc = paidUtc,
            ProviderRef = request.RazorpayPaymentId,
            Mode = request.Mode,
            Source = "Online",
            Provider = "Razorpay",
            Status = "Success"
        };
        dbContext.Payments.Add(payment);

        var profile = await dbContext.CustomerProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerProfileId, ct);
        if (profile is not null)
        {
            dbContext.PaymentModeHistory.Add(new PaymentModeHistory
            {
                TenantId = profile.TenantId,
                CustomerProfileId = profile.Id,
                InvoiceId = invoice.Id,
                PaymentId = payment.Id,
                Mode = request.Mode,
                Source = "Online",
                ReferenceId = request.RazorpayPaymentId,
                OccurredUtc = paidUtc
            });
        }

        invoice.OutstandingAmount = 0m;
        invoice.PaymentStatus = "Paid";
        invoice.LastPaymentUtc = paidUtc;
        invoice.LastPaymentMode = request.Mode;
        invoice.Status = RecordStatus.Inactive;

        var subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(x => x.CustomerProfileId == request.CustomerProfileId, ct);
        if (subscription is not null)
        {
            var cycleMonths = CycleMonths(request.Cycle);
            var today = DateTime.UtcNow.Date;
            var startDate = subscription.EndDateUtc.Date >= today ? subscription.EndDateUtc.Date.AddDays(1) : today;
            var endDate = startDate.AddMonths(cycleMonths).AddDays(-1);
            subscription.StartDateUtc = startDate;
            subscription.EndDateUtc = endDate;
            subscription.RenewalDateUtc = endDate;
            subscription.UpdatedUtc = DateTime.UtcNow;
        }

        dbContext.AuditEvents.Add(new AuditEvent
        {
            TenantId = profile?.TenantId,
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "customer",
            UserRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "CustomerAdmin",
            EventType = "Payment",
            EntityType = "Invoice",
            EntityId = invoice.Id.ToString(),
            Action = "Online payment success",
            DetailsJson = JsonSerializer.Serialize(new { request.RazorpayOrderId, request.RazorpayPaymentId, request.Cycle, invoice.InvoiceNumber }),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            OccurredUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(ct);

        return Ok(new ApiResponseDto<PaymentProcessResultDto>(true, new PaymentProcessResultDto(
            true,
            "success",
            "Payment completed successfully.",
            invoice.Id,
            invoice.InvoiceNumber,
            paidUtc)));
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(ct);
        var signature = Request.Headers["X-Razorpay-Signature"].ToString();

        var setting = await dbContext.RazorpaySettings.AsNoTracking().FirstOrDefaultAsync(x => x.TenantId == Guid.Empty && x.IsActive, ct);
        if (setting is null)
            return Ok();

        var webhookSecret = Decrypt(setting.WebhookSecretEncrypted);
        if (string.IsNullOrWhiteSpace(webhookSecret))
            return Ok();

        var expected = ComputeHmacSha256(payload, webhookSecret);
        if (!string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase))
            return Unauthorized();

        using var doc = JsonDocument.Parse(payload);
        var eventId = doc.RootElement.TryGetProperty("event", out var eventTypeEl)
            ? $"{eventTypeEl.GetString()}-{doc.RootElement.GetProperty("created_at").GetInt64()}"
            : Guid.NewGuid().ToString("N");
        var eventType = doc.RootElement.TryGetProperty("event", out var et) ? et.GetString() ?? "unknown" : "unknown";

        var exists = await dbContext.RazorpayWebhookEvents.AsNoTracking().AnyAsync(x => x.EventId == eventId, ct);
        if (exists)
            return Ok();

        dbContext.RazorpayWebhookEvents.Add(new RazorpayWebhookEvent
        {
            EventId = eventId,
            EventType = eventType,
            Signature = signature,
            Payload = payload,
            IsProcessed = true,
            ProcessedUtc = DateTime.UtcNow
        });

        await ApplyWebhookBusinessUpdatesAsync(doc.RootElement, eventType, ct);

        dbContext.AuditEvents.Add(new AuditEvent
        {
            EventType = "PaymentWebhook",
            EntityType = "RazorpayWebhook",
            EntityId = eventId,
            Action = "Processed",
            UserEmail = "system",
            UserRole = "System",
            DetailsJson = JsonSerializer.Serialize(new { eventType }),
            OccurredUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok();
    }

    private async Task ApplyWebhookBusinessUpdatesAsync(JsonElement root, string eventType, CancellationToken ct)
    {
        var normalized = (eventType ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized is "payment.captured" or "payment.authorized" or "order.paid" or "payment.failed")
        {
            if (!TryGetNested(root, out var paymentEntity, "payload", "payment", "entity"))
                return;

            var razorpayPaymentId = ReadString(paymentEntity, "id");
            if (string.IsNullOrWhiteSpace(razorpayPaymentId))
                return;

            var notes = TryGetProperty(paymentEntity, "notes", out var noteElement) ? noteElement : default;
            var invoiceId = notes.ValueKind != JsonValueKind.Undefined
                ? ParseGuid(ReadString(notes, "invoiceId"))
                : null;

            if (!invoiceId.HasValue)
                return;

            var invoice = await dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == invoiceId.Value, ct);
            if (invoice is null)
                return;

            var amountPaise = ReadLong(paymentEntity, "amount");
            var amount = decimal.Round(amountPaise / 100m, 2, MidpointRounding.AwayFromZero);
            var paidUtc = DateTime.UtcNow;
            var status = normalized == "payment.failed" ? "Failed" : "Success";

            var payment = await dbContext.Payments.FirstOrDefaultAsync(x => x.ProviderRef == razorpayPaymentId, ct);
            if (payment is null)
            {
                payment = new Payment
                {
                    InvoiceId = invoice.Id,
                    Amount = amount <= 0m ? invoice.Total : amount,
                    PaidUtc = paidUtc,
                    ProviderRef = razorpayPaymentId,
                    Mode = "Online",
                    Source = "Webhook",
                    Provider = "Razorpay",
                    Status = status,
                    Notes = $"Webhook:{eventType}"
                };
                dbContext.Payments.Add(payment);
            }
            else
            {
                payment.Status = status;
                payment.Notes = $"Webhook:{eventType}";
                payment.UpdatedUtc = DateTime.UtcNow;
            }

            if (status == "Success")
            {
                invoice.OutstandingAmount = 0m;
                invoice.PaymentStatus = "Paid";
                invoice.LastPaymentUtc = paidUtc;
                invoice.LastPaymentMode = "Online";
                invoice.Status = RecordStatus.Inactive;

                var customerProfileId = notes.ValueKind != JsonValueKind.Undefined
                    ? ParseGuid(ReadString(notes, "customerProfileId"))
                    : null;
                var cycle = notes.ValueKind != JsonValueKind.Undefined
                    ? ReadString(notes, "cycle")
                    : null;

                if (customerProfileId.HasValue && !string.IsNullOrWhiteSpace(cycle))
                {
                    var profile = await dbContext.CustomerProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerProfileId.Value, ct);
                    if (profile is not null)
                    {
                        dbContext.PaymentModeHistory.Add(new PaymentModeHistory
                        {
                            TenantId = profile.TenantId,
                            CustomerProfileId = profile.Id,
                            InvoiceId = invoice.Id,
                            PaymentId = payment.Id,
                            Mode = "Online",
                            Source = "Webhook",
                            ReferenceId = razorpayPaymentId,
                            OccurredUtc = paidUtc
                        });
                    }

                    var subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId.Value, ct);
                    if (subscription is not null)
                    {
                        var cycleMonths = CycleMonths(cycle);
                        if (cycleMonths > 0)
                        {
                            var today = DateTime.UtcNow.Date;
                            var startDate = subscription.EndDateUtc.Date >= today ? subscription.EndDateUtc.Date.AddDays(1) : today;
                            var endDate = startDate.AddMonths(cycleMonths).AddDays(-1);
                            subscription.StartDateUtc = startDate;
                            subscription.EndDateUtc = endDate;
                            subscription.RenewalDateUtc = endDate;
                            subscription.UpdatedUtc = DateTime.UtcNow;
                        }
                    }
                }
            }
            else
            {
                if (invoice.OutstandingAmount <= 0)
                    invoice.OutstandingAmount = invoice.Total;
                invoice.PaymentStatus = "Failed";
                invoice.Status = RecordStatus.Active;
                invoice.LastPaymentUtc = paidUtc;
                invoice.LastPaymentMode = "Online";
            }

            dbContext.AuditEvents.Add(new AuditEvent
            {
                EventType = "PaymentWebhook",
                EntityType = "Invoice",
                EntityId = invoice.Id.ToString(),
                Action = status == "Success" ? "PaymentCaptured" : "PaymentFailed",
                UserEmail = "system",
                UserRole = "System",
                DetailsJson = JsonSerializer.Serialize(new { eventType, razorpayPaymentId, invoice.InvoiceNumber, status }),
                OccurredUtc = DateTime.UtcNow
            });

            return;
        }

        if (normalized is "refund.processed" or "refund.created")
        {
            if (!TryGetNested(root, out var refundEntity, "payload", "refund", "entity"))
                return;

            var paymentRef = ReadString(refundEntity, "payment_id");
            if (string.IsNullOrWhiteSpace(paymentRef))
                return;

            var payment = await dbContext.Payments.FirstOrDefaultAsync(x => x.ProviderRef == paymentRef, ct);
            if (payment is null)
                return;

            var invoice = await dbContext.Invoices.FirstOrDefaultAsync(x => x.Id == payment.InvoiceId, ct);
            if (invoice is null)
                return;

            var refundAmount = decimal.Round(ReadLong(refundEntity, "amount") / 100m, 2, MidpointRounding.AwayFromZero);
            invoice.OutstandingAmount = decimal.Round(Math.Max(0m, invoice.OutstandingAmount + refundAmount), 2, MidpointRounding.AwayFromZero);
            invoice.PaymentStatus = invoice.OutstandingAmount > 0m ? "Refunded" : "Paid";
            invoice.Status = invoice.OutstandingAmount > 0m ? RecordStatus.Active : RecordStatus.Inactive;
            payment.Status = "Refunded";
            payment.UpdatedUtc = DateTime.UtcNow;
            payment.Notes = $"Webhook:{eventType}";

            dbContext.AuditEvents.Add(new AuditEvent
            {
                EventType = "PaymentWebhook",
                EntityType = "Invoice",
                EntityId = invoice.Id.ToString(),
                Action = "RefundProcessed",
                UserEmail = "system",
                UserRole = "System",
                DetailsJson = JsonSerializer.Serialize(new { eventType, paymentRef, refundAmount, invoice.InvoiceNumber }),
                OccurredUtc = DateTime.UtcNow
            });
        }
    }

    private static bool TryGetNested(JsonElement root, out JsonElement value, params string[] path)
    {
        value = root;
        foreach (var key in path)
        {
            if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty(key, out var next))
            {
                value = default;
                return false;
            }
            value = next;
        }
        return true;
    }

    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        value = default;
        return element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out value);
    }

    private static string ReadString(JsonElement element, string propertyName)
    {
        return element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null
            ? value.GetString() ?? string.Empty
            : string.Empty;
    }

    private static long ReadLong(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(propertyName, out var value))
            return 0;
        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var number))
            return number;
        if (value.ValueKind == JsonValueKind.String && long.TryParse(value.GetString(), out number))
            return number;
        return 0;
    }

    private static Guid? ParseGuid(string value)
    {
        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    private string Decrypt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var protector = protectionProvider.CreateProtector("WrkPlan.Razorpay.Settings");
        try { return protector.Unprotect(value); }
        catch { return string.Empty; }
    }

    private static int CycleMonths(string cycle)
    {
        return cycle.Trim().ToLowerInvariant() switch
        {
            "monthly" => 1,
            "quarterly" => 3,
            "annual" => 12,
            _ => 0
        };
    }

    private static string ComputeHmacSha256(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

[Authorize(Roles = "CustomerAdmin,WrkPlanAdmin")]
public class InvoicesController(AdminDbContext dbContext, TenantContext tenantContext, ILogger<InvoicesController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<InvoiceSummaryDto>>>> Get([FromQuery] PagedRequestDto request, CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        var pageNumber = request.SafePageNumber;
        var pageSize = request.SafePageSize;
        var query = dbContext.Invoices.AsNoTracking().Where(x => x.CustomerProfileId == customerProfileId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.InvoiceNumber.Contains(request.Search) ||
                x.Status.ToString().Contains(request.Search));
        }

        var sortBy = (request.SortBy ?? "createdUtc").Trim().ToLowerInvariant();
        var isDesc = request.IsDesc;
        query = sortBy switch
        {
            "amount" or "total" => isDesc ? query.OrderByDescending(x => x.Total) : query.OrderBy(x => x.Total),
            "duedate" or "dueutc" => isDesc ? query.OrderByDescending(x => x.DueUtc) : query.OrderBy(x => x.DueUtc),
            "status" => isDesc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => isDesc ? query.OrderByDescending(x => x.CreatedUtc) : query.OrderBy(x => x.CreatedUtc)
        };

        var totalCount = await query.CountAsync(ct);
        var invoices = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new InvoiceSummaryDto(x.InvoiceNumber, x.Total, x.Status.ToString(), x.DueUtc))
            .ToListAsync(ct);

        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResultDto<InvoiceSummaryDto>(pageNumber, pageSize, totalCount, totalPages, invoices);
        logger.LogInformation("Fetched invoice page {PageNumber}/{TotalPages} ({Count} items) for tenant {TenantId}", pageNumber, totalPages, invoices.Count, tenantContext.TenantId);
        return Ok(new ApiResponseDto<PagedResultDto<InvoiceSummaryDto>>(true, paged));
    }

    [HttpGet("{invoiceNumber}")]
    public async Task<ActionResult<ApiResponseDto<InvoiceDetailDto>>> Detail(string invoiceNumber, CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId && x.InvoiceNumber == invoiceNumber, ct);

        if (invoice is null)
            return NotFound(new ApiResponseDto<InvoiceDetailDto>(false, null, new ApiErrorDto("not_found", "Invoice not found.")));

        var lines = await dbContext.InvoiceItems
            .AsNoTracking()
            .Where(x => x.InvoiceId == invoice.Id)
            .Select(x => new InvoiceLineDto(x.Description, x.Amount))
            .ToListAsync(ct);

        // Fallback: build synthetic lines from subscription if no items seeded
        if (lines.Count == 0)
        {
            var sub = await dbContext.Subscriptions
                .AsNoTracking()
                .Where(x => x.CustomerProfileId == customerProfileId)
                .FirstOrDefaultAsync(ct);

            lines.Add(new InvoiceLineDto(sub is not null ? $"{sub.PlanName} — {sub.Cycle}" : "Subscription", invoice.Total));
        }

        var payment = await dbContext.Payments
            .AsNoTracking()
            .Where(x => x.InvoiceId == invoice.Id)
            .OrderByDescending(x => x.PaidUtc)
            .FirstOrDefaultAsync(ct);

        var sub2 = await dbContext.Subscriptions
            .AsNoTracking()
            .Where(x => x.CustomerProfileId == customerProfileId)
            .FirstOrDefaultAsync(ct);

        var detail = new InvoiceDetailDto(
            invoice.Id, invoice.InvoiceNumber,
            sub2?.PlanName ?? "Subscription",
            sub2?.Cycle.ToString() ?? "Monthly",
            invoice.Total,
            0m, 0m,
            invoice.Total,
            invoice.Status.ToString(),
            invoice.DueUtc,
            payment?.PaidUtc,
            payment?.ProviderRef,
            lines);

        logger.LogInformation("Fetched invoice detail {InvoiceNumber} for tenant {TenantId}", invoiceNumber, tenantContext.TenantId);
        return Ok(new ApiResponseDto<InvoiceDetailDto>(true, detail));
    }

    [HttpGet("{invoiceNumber}/pdf")]
    public async Task<IActionResult> DownloadPdf(string invoiceNumber, CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId && x.InvoiceNumber == invoiceNumber, ct);

        if (invoice is null)
            return NotFound();

        // Minimal PDF-like payload to provide a reliable downloadable file in demo mode.
        var content = $"WrkPlan Invoice\\nInvoice: {invoice.InvoiceNumber}\\nAmount: {invoice.Total:C}\\nDue: {invoice.DueUtc:yyyy-MM-dd}\\nStatus: {invoice.Status}";
        var bytes = Encoding.UTF8.GetBytes(content);
        logger.LogInformation("Generated downloadable invoice payload for {InvoiceNumber}", invoiceNumber);
        return File(bytes, "application/pdf", $"{invoice.InvoiceNumber}.pdf");
    }
}

// ── Admin Subscription Plans ───────────────────────────────────────────────
[Authorize(Roles = "WrkPlanAdmin")]
public class AdminSubscriptionPlansController(
    AdminDbContext dbContext,
    ILogger<AdminSubscriptionPlansController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<SubscriptionPlanDto>>>> List(CancellationToken ct)
    {
        var entities = await dbContext.SubscriptionPlans.AsNoTracking()
            .OrderBy(x => x.Price)
            .ToListAsync(ct);

        var plans = entities.Select(x => new SubscriptionPlanDto(
            x.Id, x.Name, x.Description, x.Cycle.ToString(), x.Price, x.IsActive,
            System.Text.Json.JsonSerializer.Deserialize<List<string>>(x.FeaturesJson) ?? new())).ToList();

        return Ok(new ApiResponseDto<List<SubscriptionPlanDto>>(true, plans));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<SubscriptionPlanDto>>> Create(
        [FromBody] CreateSubscriptionPlanDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new ApiResponseDto<SubscriptionPlanDto>(false, null,
                new ApiErrorDto("validation", "Plan name is required.")));

        if (!Enum.TryParse<SubscriptionCycle>(dto.Cycle, true, out var cycle))
            return BadRequest(new ApiResponseDto<SubscriptionPlanDto>(false, null,
                new ApiErrorDto("validation", "Invalid billing cycle. Use Monthly, Quarterly, or Annual.")));

        var plan = new SubscriptionPlan
        {
            Name = dto.Name, Description = dto.Description,
            Cycle = cycle, Price = dto.Price,
            FeaturesJson = System.Text.Json.JsonSerializer.Serialize(dto.Features ?? new()),
            IsActive = true
        };
        dbContext.SubscriptionPlans.Add(plan);
        await dbContext.SaveChangesAsync(ct);
        logger.LogInformation("Created subscription plan {PlanName}", plan.Name);

        var result = new SubscriptionPlanDto(plan.Id, plan.Name, plan.Description, plan.Cycle.ToString(), plan.Price, plan.IsActive, dto.Features ?? new());
        return Ok(new ApiResponseDto<SubscriptionPlanDto>(true, result));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<string>>> Update(
        Guid id, [FromBody] UpdateSubscriptionPlanDto dto, CancellationToken ct)
    {
        var plan = await dbContext.SubscriptionPlans.FindAsync([id], ct);
        if (plan is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Plan not found.")));

        if (!Enum.TryParse<SubscriptionCycle>(dto.Cycle, true, out var cycle))
            return BadRequest(new ApiResponseDto<string>(false, null,
                new ApiErrorDto("validation", "Invalid billing cycle.")));

        plan.Name = dto.Name;
        plan.Description = dto.Description;
        plan.Cycle = cycle;
        plan.Price = dto.Price;
        plan.IsActive = dto.IsActive;
        plan.FeaturesJson = System.Text.Json.JsonSerializer.Serialize(dto.Features ?? new());
        plan.UpdatedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
        logger.LogInformation("Updated subscription plan {PlanId}", id);
        return Ok(new ApiResponseDto<string>(true, "updated"));
    }

    [HttpPatch("{id:guid}/toggle")]
    public async Task<ActionResult<ApiResponseDto<string>>> Toggle(Guid id, CancellationToken ct)
    {
        var plan = await dbContext.SubscriptionPlans.FindAsync([id], ct);
        if (plan is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Plan not found.")));
        plan.IsActive = !plan.IsActive;
        plan.UpdatedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, plan.IsActive ? "activated" : "deactivated"));
    }

    [HttpPost("assign")]
    public async Task<ActionResult<ApiResponseDto<string>>> AssignToTenant(
        [FromBody] AssignSubscriptionPlanDto dto, CancellationToken ct)
    {
        var tenant = await dbContext.TenantRegistries.FindAsync([dto.TenantId], ct);
        if (tenant is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Tenant not found.")));

        var plan = await dbContext.SubscriptionPlans.FindAsync([dto.PlanId], ct);
        if (plan is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Plan not found.")));

        var profile = await dbContext.CustomerProfiles
            .FirstOrDefaultAsync(x => x.TenantId == dto.TenantId, ct);
        if (profile is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Customer profile not found.")));

        var existing = await dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.CustomerProfileId == profile.Id, ct);

        var oldPlan = existing?.PlanName ?? "None";

        if (existing is not null)
        {
            existing.PlanName = plan.Name;
            existing.Price = plan.Price;
            existing.Cycle = plan.Cycle;
            existing.StartDateUtc = dto.EffectiveDate.ToUniversalTime();
            existing.EndDateUtc = dto.EffectiveDate.AddYears(1).ToUniversalTime();
            existing.RenewalDateUtc = dto.EffectiveDate.AddYears(1).ToUniversalTime();
            existing.UpdatedUtc = DateTime.UtcNow;
        }
        else
        {
            dbContext.Subscriptions.Add(new Subscription
            {
                CustomerProfileId = profile.Id,
                PlanName = plan.Name, Price = plan.Price, Cycle = plan.Cycle,
                StartDateUtc = dto.EffectiveDate.ToUniversalTime(),
                EndDateUtc = dto.EffectiveDate.AddYears(1).ToUniversalTime(),
                RenewalDateUtc = dto.EffectiveDate.AddYears(1).ToUniversalTime(),
                AutoRenew = true
            });
        }

        dbContext.SubscriptionChangeLogs.Add(new SubscriptionChangeLog
        {
            TenantId = dto.TenantId,
            CustomerProfileId = profile.Id,
            OldPlanName = oldPlan,
            NewPlanName = plan.Name,
            ChangedBy = User?.Identity?.Name ?? "admin",
            Notes = dto.Notes
        });

        await dbContext.SaveChangesAsync(ct);
        logger.LogInformation("Assigned plan {PlanName} to tenant {TenantId} (was {OldPlan})", plan.Name, dto.TenantId, oldPlan);
        return Ok(new ApiResponseDto<string>(true, $"Assigned {plan.Name} to {tenant.Name}"));
    }

    [HttpGet("change-log")]
    public async Task<ActionResult<ApiResponseDto<List<SubscriptionChangeLogDto>>>> ChangeLog(CancellationToken ct)
    {
        var logs = await dbContext.SubscriptionChangeLogs.AsNoTracking()
            .OrderByDescending(x => x.CreatedUtc)
            .Take(50)
            .ToListAsync(ct);

        var tenantIds = logs.Select(x => x.TenantId).Distinct().ToList();
        var tenants = await dbContext.TenantRegistries.AsNoTracking()
            .Where(x => tenantIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        var result = logs.Select(x => new SubscriptionChangeLogDto(
            x.Id, tenants.GetValueOrDefault(x.TenantId, "Unknown"),
            x.OldPlanName, x.NewPlanName, x.ChangedBy, x.CreatedUtc, x.Notes))
            .ToList();

        return Ok(new ApiResponseDto<List<SubscriptionChangeLogDto>>(true, result));
    }
}

// ── Customer subscription view ─────────────────────────────────────────────
[Authorize(Roles = "CustomerAdmin")]
public class CustomerSubscriptionController(
    AdminDbContext dbContext, TenantContext tenantContext,
    ILogger<CustomerSubscriptionController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<CustomerSubscriptionDto>>> Get(CancellationToken ct)
    {
        var profile = await dbContext.CustomerProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantContext.TenantId, ct);

        if (profile is null)
            return NotFound(new ApiResponseDto<CustomerSubscriptionDto>(false, null,
                new ApiErrorDto("not_found", "Customer profile not found.")));

        var sub = await dbContext.Subscriptions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerProfileId == profile.Id, ct);

        if (sub is null)
            return NotFound(new ApiResponseDto<CustomerSubscriptionDto>(false, null,
                new ApiErrorDto("not_found", "No active subscription found.")));

        // Try to get matching plan for features
        var plan = await dbContext.SubscriptionPlans.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == sub.PlanName && x.IsActive, ct);

        var features = plan is not null
            ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(plan.FeaturesJson) ?? new()
            : new List<string>();

        var status = sub.RenewalDateUtc < DateTime.UtcNow ? "Overdue" :
                     sub.RenewalDateUtc < DateTime.UtcNow.AddDays(7) ? "RenewalDue" : "Active";

        var dto = new CustomerSubscriptionDto(
            sub.Id, profile.Id, sub.PlanName, plan?.Description ?? string.Empty,
            sub.Cycle.ToString(), sub.Price,
            sub.StartDateUtc, sub.EndDateUtc, sub.RenewalDateUtc,
            sub.AutoRenew, status, features);

        logger.LogInformation("Customer subscription fetched for tenant {TenantId}", tenantContext.TenantId);
        return Ok(new ApiResponseDto<CustomerSubscriptionDto>(true, dto));
    }
}

[Authorize(Roles = "CustomerAdmin,WrkPlanAdmin")]
public class DashboardController(IDashboardService dashboardService, TenantContext tenantContext, AdminDbContext dbContext) : BaseApiController
{
    [HttpGet("portal")]
    public async Task<ActionResult<ApiResponseDto<DashboardResponseDto>>> Portal(CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (customerProfileId == Guid.Empty)
        {
            return NotFound(new ApiResponseDto<DashboardResponseDto>(false, null, new ApiErrorDto("profile_not_found", "Customer profile not found for tenant.")));
        }

        var dashboard = await dashboardService.GetCustomerPortalDashboardAsync(customerProfileId, ct);
        return Ok(new ApiResponseDto<DashboardResponseDto>(true, dashboard));
    }
}

[Authorize(Roles = "CustomerAdmin,WrkPlanAdmin")]
public class SupportTicketsController(AdminDbContext dbContext, TenantContext tenantContext, ILogger<SupportTicketsController> logger) : BaseApiController
{
    private static readonly HashSet<string> AllowedTicketStatuses =
    ["Open", "In Progress", "Awaiting Customer", "Resolved", "Closed"];

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<SupportTicketDto>>>> Get([FromQuery] PagedRequestDto request, CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        var pageNumber = request.SafePageNumber;
        var pageSize = request.SafePageSize;
        var query = dbContext.SupportTickets.AsNoTracking().Where(x => x.CustomerProfileId == customerProfileId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Subject.Contains(request.Search) ||
                x.Priority.Contains(request.Search) ||
                x.WorkflowStatus.Contains(request.Search) ||
                x.TicketNumber.Contains(request.Search));
        }

        var sortBy = (request.SortBy ?? "createdUtc").Trim().ToLowerInvariant();
        var isDesc = request.IsDesc;
        query = sortBy switch
        {
            "priority" => isDesc ? query.OrderByDescending(x => x.Priority) : query.OrderBy(x => x.Priority),
            "status" => isDesc ? query.OrderByDescending(x => x.WorkflowStatus) : query.OrderBy(x => x.WorkflowStatus),
            _ => isDesc ? query.OrderByDescending(x => x.CreatedUtc) : query.OrderBy(x => x.CreatedUtc)
        };

        var totalCount = await query.CountAsync(ct);
        var tickets = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SupportTicketDto(
                x.Id,
                x.Subject,
                x.Priority,
                x.WorkflowStatus,
                x.TicketNumber,
                x.CreatedUtc,
                x.ResolvedUtc,
                x.ResolvedInDays,
                x.ResolutionMessage,
                null))
            .ToListAsync(ct);

        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResultDto<SupportTicketDto>(pageNumber, pageSize, totalCount, totalPages, tickets);
        return Ok(new ApiResponseDto<PagedResultDto<SupportTicketDto>>(true, paged));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<SupportTicketDto>>> Create([FromBody] CreateSupportTicketDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Subject))
        {
            return BadRequest(new ApiResponseDto<SupportTicketDto>(false, null,
                new ApiErrorDto("validation", "Subject is required.")));
        }

        var priority = string.IsNullOrWhiteSpace(request.Priority) ? "Normal" : request.Priority.Trim();
        if (priority is not ("Low" or "Normal" or "High" or "Critical"))
        {
            return BadRequest(new ApiResponseDto<SupportTicketDto>(false, null,
                new ApiErrorDto("validation", "Priority must be Low, Normal, High, or Critical.")));
        }

        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (customerProfileId == Guid.Empty)
        {
            return NotFound(new ApiResponseDto<SupportTicketDto>(false, null,
                new ApiErrorDto("profile_not_found", "Customer profile not found for tenant.")));
        }

        var ticket = new SupportTicket
        {
            CustomerProfileId = customerProfileId,
            TicketNumber = await NextTicketNumberAsync(ct),
            Subject = request.Subject.Trim(),
            Priority = priority,
            Status = RecordStatus.Active,
            WorkflowStatus = "Open"
        };

        dbContext.SupportTickets.Add(ticket);
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            dbContext.TicketMessages.Add(new TicketMessage
            {
                SupportTicketId = ticket.Id,
                Message = request.Description.Trim(),
                AuthorRole = "Customer",
                AuthorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "customer"
            });
        }

        dbContext.TicketStatusHistories.Add(new TicketStatusHistory
        {
            SupportTicketId = ticket.Id,
            FromStatus = "",
            ToStatus = "Open",
            ChangedBy = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "customer",
            ChangedByRole = "Customer",
            Notes = "Ticket created"
        });

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Support ticket created {TicketId} for tenant {TenantId}", ticket.Id, tenantContext.TenantId);
        var response = new SupportTicketDto(ticket.Id, ticket.Subject, ticket.Priority, ticket.WorkflowStatus, ticket.TicketNumber, ticket.CreatedUtc, null, null, null, null);
        return Ok(new ApiResponseDto<SupportTicketDto>(true, response));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<SupportTicketDetailDto>>> Detail(Guid id, CancellationToken ct)
    {
        var ticket = await dbContext.SupportTickets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (ticket is null)
            return NotFound(new ApiResponseDto<SupportTicketDetailDto>(false, null, new ApiErrorDto("not_found", "Ticket not found.")));

        if (User.IsInRole("CustomerAdmin"))
        {
            var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
            if (ticket.CustomerProfileId != customerProfileId)
                return Forbid();
        }

        var messages = await dbContext.TicketMessages.AsNoTracking()
            .Where(x => x.SupportTicketId == id)
            .OrderBy(x => x.CreatedUtc)
            .Select(x => new SupportTicketMessageDto(x.Id, x.Message, x.AuthorRole, x.AuthorEmail, x.CreatedUtc, x.IsResolution))
            .ToListAsync(ct);

        var history = await dbContext.TicketStatusHistories.AsNoTracking()
            .Where(x => x.SupportTicketId == id)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new SupportTicketStatusHistoryDto(x.Id, x.FromStatus, x.ToStatus, x.ChangedBy, x.ChangedByRole, x.CreatedUtc, x.Notes))
            .ToListAsync(ct);

        string? tenantName = null;
        if (User.IsInRole("WrkPlanAdmin"))
        {
            var tenantId = await dbContext.CustomerProfiles.AsNoTracking()
                .Where(x => x.Id == ticket.CustomerProfileId)
                .Select(x => x.TenantId)
                .FirstOrDefaultAsync(ct);
            tenantName = await dbContext.TenantRegistries.AsNoTracking()
                .Where(x => x.Id == tenantId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(ct);
        }

        var detail = new SupportTicketDetailDto(
            ticket.Id,
            ticket.TicketNumber,
            ticket.Subject,
            ticket.Priority,
            ticket.WorkflowStatus,
            tenantName,
            ticket.CreatedUtc,
            ticket.ResolvedUtc,
            ticket.ResolvedInDays,
            ticket.ResolutionMessage,
            messages,
            history);

        return Ok(new ApiResponseDto<SupportTicketDetailDto>(true, detail));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("admin/list")]
    public async Task<ActionResult<ApiResponseDto<AdminSupportTicketListDto>>> AdminList(
        [FromBody] AdminSupportTicketFilterDto request,
        CancellationToken ct)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 200 ? 200 : request.PageSize;

        var query = dbContext.SupportTickets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Subject.Contains(request.Search) ||
                x.TicketNumber.Contains(request.Search) ||
                x.WorkflowStatus.Contains(request.Search));
        }
        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(x => x.WorkflowStatus == request.Status);
        if (!string.IsNullOrWhiteSpace(request.Priority))
            query = query.Where(x => x.Priority == request.Priority);

        if (request.TenantId.HasValue)
        {
            var profileIds = await dbContext.CustomerProfiles.AsNoTracking()
                .Where(x => x.TenantId == request.TenantId.Value)
                .Select(x => x.Id)
                .ToListAsync(ct);
            query = query.Where(x => profileIds.Contains(x.CustomerProfileId));
        }

        if (request.FromUtc.HasValue)
            query = query.Where(x => x.CreatedUtc >= request.FromUtc.Value);
        if (request.ToUtc.HasValue)
            query = query.Where(x => x.CreatedUtc <= request.ToUtc.Value);

        var totalCount = await query.CountAsync(ct);
        var source = await query.OrderByDescending(x => x.CreatedUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var profileMap = await dbContext.CustomerProfiles.AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x.TenantId, ct);
        var tenantIds = profileMap.Values.Distinct().ToList();
        var tenantNames = await dbContext.TenantRegistries.AsNoTracking()
            .Where(x => tenantIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        var tickets = source.Select(x =>
        {
            var tenantName = profileMap.TryGetValue(x.CustomerProfileId, out var tenantId) && tenantNames.TryGetValue(tenantId, out var tName)
                ? tName
                : null;
            return new SupportTicketDto(
                x.Id,
                x.Subject,
                x.Priority,
                x.WorkflowStatus,
                x.TicketNumber,
                x.CreatedUtc,
                x.ResolvedUtc,
                x.ResolvedInDays,
                x.ResolutionMessage,
                tenantName);
        }).ToList();

        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResultDto<SupportTicketDto>(pageNumber, pageSize, totalCount, totalPages, tickets);

        var openCount = await dbContext.SupportTickets.CountAsync(x => x.WorkflowStatus == "Open", ct);
        var inProgressCount = await dbContext.SupportTickets.CountAsync(x => x.WorkflowStatus == "In Progress", ct);
        var resolvedCount = await dbContext.SupportTickets.CountAsync(x => x.WorkflowStatus == "Resolved", ct);
        var overdueCount = await dbContext.SupportTickets.CountAsync(x =>
            (x.WorkflowStatus == "Open" || x.WorkflowStatus == "In Progress") && x.CreatedUtc < DateTime.UtcNow.AddDays(-7), ct);

        var result = new AdminSupportTicketListDto(paged, new SupportTicketCountersDto(openCount, inProgressCount, resolvedCount, overdueCount));
        return Ok(new ApiResponseDto<AdminSupportTicketListDto>(true, result));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("admin/reply")]
    public async Task<ActionResult<ApiResponseDto<string>>> AdminReply([FromBody] AdminReplyTicketDto request, CancellationToken ct)
    {
        var ticket = await dbContext.SupportTickets.FirstOrDefaultAsync(x => x.Id == request.TicketId, ct);
        if (ticket is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Ticket not found.")));

        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Reply message is required.")));

        dbContext.TicketMessages.Add(new TicketMessage
        {
            SupportTicketId = ticket.Id,
            Message = request.Message.Trim(),
            AuthorRole = "WrkPlanAdmin",
            AuthorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin"
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "reply_saved"));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("admin/status")]
    public async Task<ActionResult<ApiResponseDto<string>>> ChangeStatus([FromBody] ChangeTicketStatusDto request, CancellationToken ct)
    {
        var ticket = await dbContext.SupportTickets.FirstOrDefaultAsync(x => x.Id == request.TicketId, ct);
        if (ticket is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Ticket not found.")));

        var next = NormalizeTicketStatus(request.Status);
        if (!AllowedTicketStatuses.Contains(next))
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Invalid status.")));

        var prev = ticket.WorkflowStatus;
        ticket.WorkflowStatus = next;
        ticket.UpdatedUtc = DateTime.UtcNow;

        dbContext.TicketStatusHistories.Add(new TicketStatusHistory
        {
            SupportTicketId = ticket.Id,
            FromStatus = prev,
            ToStatus = next,
            ChangedBy = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            ChangedByRole = "WrkPlanAdmin",
            Notes = request.Notes
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, next));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("admin/resolve")]
    public async Task<ActionResult<ApiResponseDto<string>>> Resolve([FromBody] ResolveTicketDto request, CancellationToken ct)
    {
        var ticket = await dbContext.SupportTickets.FirstOrDefaultAsync(x => x.Id == request.TicketId, ct);
        if (ticket is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Ticket not found.")));
        if (string.IsNullOrWhiteSpace(request.ResolutionMessage))
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("validation", "Resolution message is required.")));

        var resolvedAt = DateTime.UtcNow;
        ticket.ResolvedUtc = resolvedAt;
        ticket.ResolvedInDays = Math.Max(0, (int)Math.Ceiling((resolvedAt - ticket.CreatedUtc).TotalDays));
        ticket.ResolutionMessage = request.ResolutionMessage.Trim();
        var prev = ticket.WorkflowStatus;
        ticket.WorkflowStatus = "Resolved";
        ticket.UpdatedUtc = DateTime.UtcNow;

        dbContext.TicketMessages.Add(new TicketMessage
        {
            SupportTicketId = ticket.Id,
            Message = request.ResolutionMessage.Trim(),
            AuthorRole = "WrkPlanAdmin",
            AuthorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            IsResolution = true
        });

        dbContext.TicketStatusHistories.Add(new TicketStatusHistory
        {
            SupportTicketId = ticket.Id,
            FromStatus = prev,
            ToStatus = "Resolved",
            ChangedBy = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            ChangedByRole = "WrkPlanAdmin",
            Notes = "Resolved with message"
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "resolved"));
    }

    private async Task<Guid> ResolveCustomerProfileIdAsync(CancellationToken ct)
    {
        return await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);
    }

    private static string NormalizeTicketStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return "Open";
        return AllowedTicketStatuses.FirstOrDefault(x => x.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase)) ?? "Open";
    }

    private async Task<string> NextTicketNumberAsync(CancellationToken ct)
    {
        var latest = await dbContext.SupportTickets
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => x.TicketNumber)
            .FirstOrDefaultAsync(ct);

        var seq = 1;
        if (!string.IsNullOrWhiteSpace(latest))
        {
            var tail = latest.Split('-').LastOrDefault();
            if (int.TryParse(tail, out var parsed))
                seq = parsed + 1;
        }
        return $"TKT-{DateTime.UtcNow:yyyyMMdd}-{seq:0000}";
    }
}

[Authorize(Roles = "CustomerAdmin,WrkPlanAdmin")]
public class AnnouncementsController(AdminDbContext dbContext, TenantContext tenantContext) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<AnnouncementDto>>>> Get([FromQuery] PagedRequestDto request, CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        var tenantId = await dbContext.CustomerProfiles
            .Where(x => x.Id == customerProfileId)
            .Select(x => x.TenantId)
            .FirstOrDefaultAsync(ct);

        var pageNumber = request.SafePageNumber;
        var pageSize = request.SafePageSize;
        var targetedIds = await dbContext.AnnouncementTenantTargets.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.AnnouncementId)
            .ToListAsync(ct);

        var query = dbContext.Announcements.AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.IsPublished &&
                (x.IsGlobal || x.TenantId == tenantId || x.CustomerProfileId == customerProfileId || targetedIds.Contains(x.Id)));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Title.Contains(request.Search) ||
                x.Message.Contains(request.Search) ||
                x.Topic.Contains(request.Search));
        }

        var sortBy = (request.SortBy ?? "createdUtc").Trim().ToLowerInvariant();
        var isDesc = request.IsDesc;
        query = sortBy switch
        {
            "title" => isDesc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
            _ => isDesc ? query.OrderByDescending(x => x.CreatedUtc) : query.OrderBy(x => x.CreatedUtc)
        };

        var totalCount = await query.CountAsync(ct);
        var pageRows = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        var pageAnnouncementIds = pageRows.Select(x => x.Id).ToList();
        var targets = await dbContext.AnnouncementTenantTargets.AsNoTracking()
            .Where(x => pageAnnouncementIds.Contains(x.AnnouncementId))
            .GroupBy(x => x.AnnouncementId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.TenantId).ToList(), ct);

        var announcements = pageRows
            .Select(x => new AnnouncementDto(
                x.Id,
                x.Title,
                x.Message,
                x.Topic,
                x.IsPublished,
                x.PublishedUtc,
                x.IsGlobal,
                targets.ContainsKey(x.Id) ? targets[x.Id] : new List<Guid>()))
            .ToList();

        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResultDto<AnnouncementDto>(pageNumber, pageSize, totalCount, totalPages, announcements);
        return Ok(new ApiResponseDto<PagedResultDto<AnnouncementDto>>(true, paged));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("admin/list")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<AnnouncementDto>>>> AdminList([FromBody] AnnouncementFilterDto request, CancellationToken ct)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 200 ? 200 : request.PageSize;

        IQueryable<Announcement> query = dbContext.Announcements.AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Title.Contains(request.Search) ||
                x.Message.Contains(request.Search) ||
                x.Topic.Contains(request.Search));
        }
        if (!string.IsNullOrWhiteSpace(request.Topic))
            query = query.Where(x => x.Topic == request.Topic);
        if (request.IsPublished.HasValue)
            query = query.Where(x => x.IsPublished == request.IsPublished.Value);
        if (request.FromUtc.HasValue)
            query = query.Where(x => x.CreatedUtc >= request.FromUtc.Value);
        if (request.ToUtc.HasValue)
            query = query.Where(x => x.CreatedUtc <= request.ToUtc.Value);

        query = query.OrderByDescending(x => x.CreatedUtc);
        var totalCount = await query.CountAsync(ct);
        var rows = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var ids = rows.Select(x => x.Id).ToList();
        var targets = await dbContext.AnnouncementTenantTargets.AsNoTracking()
            .Where(x => ids.Contains(x.AnnouncementId))
            .GroupBy(x => x.AnnouncementId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.TenantId).ToList(), ct);

        var items = rows.Select(x => new AnnouncementDto(
            x.Id,
            x.Title,
            x.Message,
            x.Topic,
            x.IsPublished,
            x.PublishedUtc,
            x.IsGlobal,
            targets.ContainsKey(x.Id) ? targets[x.Id] : [])).ToList();

        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        return Ok(new ApiResponseDto<PagedResultDto<AnnouncementDto>>(true,
            new PagedResultDto<AnnouncementDto>(pageNumber, pageSize, totalCount, totalPages, items)));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("admin/upsert")]
    public async Task<ActionResult<ApiResponseDto<AnnouncementDto>>> Upsert([FromBody] UpsertAnnouncementDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new ApiResponseDto<AnnouncementDto>(false, null, new ApiErrorDto("validation", "Title and message are required.")));

        var profileId = await dbContext.CustomerProfiles.AsNoTracking()
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        Announcement? entity;
        if (request.Id.HasValue)
        {
            entity = await dbContext.Announcements.FirstOrDefaultAsync(x => x.Id == request.Id.Value && !x.IsDeleted, ct);
            if (entity is null)
            {
                return NotFound(new ApiResponseDto<AnnouncementDto>(false, null, new ApiErrorDto("not_found", "Announcement not found.")));
            }
            entity.Title = request.Title.Trim();
            entity.Message = request.Message.Trim();
            entity.Topic = request.Topic?.Trim() ?? string.Empty;
            entity.IsPublished = request.IsPublished;
            entity.IsGlobal = request.IsGlobal;
            entity.PublishedUtc = request.IsPublished ? (entity.PublishedUtc ?? DateTime.UtcNow) : null;
            entity.UpdatedUtc = DateTime.UtcNow;
        }
        else
        {
            entity = new Announcement
            {
                CustomerProfileId = profileId,
                TenantId = tenantContext.TenantId,
                Title = request.Title.Trim(),
                Message = request.Message.Trim(),
                Topic = request.Topic?.Trim() ?? string.Empty,
                IsPublished = request.IsPublished,
                IsGlobal = request.IsGlobal,
                PublishedUtc = request.IsPublished ? DateTime.UtcNow : null
            };
            dbContext.Announcements.Add(entity);
        }

        var existingTargets = await dbContext.AnnouncementTenantTargets.Where(x => x.AnnouncementId == entity.Id).ToListAsync(ct);
        dbContext.AnnouncementTenantTargets.RemoveRange(existingTargets);
        if (!request.IsGlobal && request.TenantIds is not null)
        {
            foreach (var tenantId in request.TenantIds.Distinct())
            {
                dbContext.AnnouncementTenantTargets.Add(new AnnouncementTenantTarget { AnnouncementId = entity.Id, TenantId = tenantId });
            }
        }

        dbContext.AuditEvents.Add(new AuditEvent
        {
            TenantId = tenantContext.TenantId,
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            UserRole = "WrkPlanAdmin",
            EventType = "Announcement",
            EntityType = "Announcement",
            EntityId = entity.Id.ToString(),
            Action = request.Id.HasValue ? "Updated" : "Created",
            DetailsJson = JsonSerializer.Serialize(new { entity.Title, entity.Topic, entity.IsPublished, entity.IsGlobal }),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            OccurredUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(ct);
        var targetIds = await dbContext.AnnouncementTenantTargets.AsNoTracking()
            .Where(x => x.AnnouncementId == entity.Id)
            .Select(x => x.TenantId)
            .ToListAsync(ct);

        return Ok(new ApiResponseDto<AnnouncementDto>(true, new AnnouncementDto(
            entity.Id,
            entity.Title,
            entity.Message,
            entity.Topic,
            entity.IsPublished,
            entity.PublishedUtc,
            entity.IsGlobal,
            targetIds)));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("admin/{id:guid}/publish")]
    public async Task<ActionResult<ApiResponseDto<string>>> Publish(Guid id, [FromQuery] bool publish, CancellationToken ct)
    {
        var entity = await dbContext.Announcements.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Announcement not found.")));

        entity.IsPublished = publish;
        entity.PublishedUtc = publish ? DateTime.UtcNow : null;
        entity.UpdatedUtc = DateTime.UtcNow;

        dbContext.AuditEvents.Add(new AuditEvent
        {
            TenantId = tenantContext.TenantId,
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            UserRole = "WrkPlanAdmin",
            EventType = "Announcement",
            EntityType = "Announcement",
            EntityId = entity.Id.ToString(),
            Action = publish ? "Published" : "Unpublished",
            DetailsJson = JsonSerializer.Serialize(new { entity.Title }),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            OccurredUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, publish ? "published" : "unpublished"));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpDelete("admin/{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<string>>> Delete(Guid id, CancellationToken ct)
    {
        var entity = await dbContext.Announcements.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Announcement not found.")));

        entity.IsDeleted = true;
        entity.IsPublished = false;
        entity.UpdatedUtc = DateTime.UtcNow;

        dbContext.AuditEvents.Add(new AuditEvent
        {
            TenantId = tenantContext.TenantId,
            UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin",
            UserRole = "WrkPlanAdmin",
            EventType = "Announcement",
            EntityType = "Announcement",
            EntityId = entity.Id.ToString(),
            Action = "Deleted",
            DetailsJson = JsonSerializer.Serialize(new { entity.Title }),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            OccurredUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "deleted"));
    }
}

[Authorize]
public class KnowledgeBaseController : BaseApiController
{
    [HttpGet]
    public IActionResult Get() => Ok(new ApiResponseDto<string>(true, "Knowledge base endpoint ready"));
}

[Authorize(Roles = "CustomerAdmin,WrkPlanAdmin")]
public class OnboardingController(IOnboardingEngineService onboardingEngineService) : BaseApiController
{
    [HttpPost("progress")]
    public ActionResult<ApiResponseDto<decimal>> Progress([FromBody] List<OnboardingStepDto> steps)
        => Ok(new ApiResponseDto<decimal>(true, onboardingEngineService.CalculateProgressPercent(steps)));
}

[Authorize]
public class QuestionnaireController(AdminDbContext dbContext, TenantContext tenantContext) : BaseApiController
{
    private const string TemplatePrefix = "TEMPLATE:";

    [HttpGet("template")]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> Template(CancellationToken ct)
    {
        var template = await LoadTemplateAsync(ct);
        return Ok(new ApiResponseDto<List<string>>(true, template.OrderBy(x => x.SortOrder).Select(x => x.QuestionText).ToList()));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpGet("template-items")]
    public async Task<ActionResult<ApiResponseDto<List<QuestionnaireTemplateItemDto>>>> TemplateItems(CancellationToken ct)
    {
        var template = await LoadTemplateAsync(ct);
        return Ok(new ApiResponseDto<List<QuestionnaireTemplateItemDto>>(true, template.OrderBy(x => x.SortOrder).ToList()));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpPost("template-items")]
    public async Task<ActionResult<ApiResponseDto<QuestionnaireTemplateItemDto>>> CreateTemplateItem(
        [FromBody] UpsertQuestionnaireTemplateDto request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return BadRequest(new ApiResponseDto<QuestionnaireTemplateItemDto>(false, null,
                new ApiErrorDto("validation", "Question text is required.")));
        }

        var id = Guid.NewGuid();
        var payload = JsonSerializer.Serialize(new QuestionnaireTemplatePayload(
            request.QuestionText.Trim(), request.IsRequired, request.SortOrder));

        dbContext.QuestionnaireResponses.Add(new QuestionnaireResponse
        {
            CustomerProfileId = Guid.Empty,
            QuestionKey = $"{TemplatePrefix}{id}",
            ResponseValue = payload
        });
        await dbContext.SaveChangesAsync(ct);

        var dto = new QuestionnaireTemplateItemDto(id, request.QuestionText.Trim(), request.IsRequired, request.SortOrder);
        return Ok(new ApiResponseDto<QuestionnaireTemplateItemDto>(true, dto));
    }

    [Authorize(Roles = "WrkPlanAdmin")]
    [HttpDelete("template-items/{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<string>>> DeleteTemplateItem(Guid id, CancellationToken ct)
    {
        var key = $"{TemplatePrefix}{id}";
        var entity = await dbContext.QuestionnaireResponses.FirstOrDefaultAsync(x => x.CustomerProfileId == Guid.Empty && x.QuestionKey == key, ct);
        if (entity is null)
            return NotFound(new ApiResponseDto<string>(false, null, new ApiErrorDto("not_found", "Template item not found.")));

        dbContext.QuestionnaireResponses.Remove(entity);
        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "deleted"));
    }

    [Authorize(Roles = "CustomerAdmin")]
    [HttpPost("responses")]
    public async Task<ActionResult<ApiResponseDto<string>>> SaveResponses(
        [FromBody] SubmitQuestionnaireResponseDto request,
        CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (customerProfileId == Guid.Empty)
        {
            return NotFound(new ApiResponseDto<string>(false, null,
                new ApiErrorDto("profile_not_found", "Customer profile not found for tenant.")));
        }

        if (request.Answers is null || request.Answers.Count == 0)
        {
            return BadRequest(new ApiResponseDto<string>(false, null,
                new ApiErrorDto("validation", "At least one questionnaire answer is required.")));
        }

        foreach (var answer in request.Answers.Where(x => !string.IsNullOrWhiteSpace(x.QuestionKey)))
        {
            var key = answer.QuestionKey.Trim();
            var existing = await dbContext.QuestionnaireResponses
                .FirstOrDefaultAsync(x => x.CustomerProfileId == customerProfileId && x.QuestionKey == key, ct);

            if (existing is null)
            {
                dbContext.QuestionnaireResponses.Add(new QuestionnaireResponse
                {
                    CustomerProfileId = customerProfileId,
                    QuestionKey = key,
                    ResponseValue = answer.ResponseValue?.Trim() ?? string.Empty
                });
            }
            else
            {
                existing.ResponseValue = answer.ResponseValue?.Trim() ?? string.Empty;
                existing.UpdatedUtc = DateTime.UtcNow;
            }
        }

        await dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponseDto<string>(true, "saved"));
    }

    private async Task<List<QuestionnaireTemplateItemDto>> LoadTemplateAsync(CancellationToken ct)
    {
        var rows = await dbContext.QuestionnaireResponses
            .AsNoTracking()
            .Where(x => x.CustomerProfileId == Guid.Empty && x.QuestionKey.StartsWith(TemplatePrefix))
            .ToListAsync(ct);

        if (rows.Count == 0)
        {
            return Enumerable.Range(1, 8)
                .Select(i => new QuestionnaireTemplateItemDto(Guid.NewGuid(), $"Implementation question {i}", true, i))
                .ToList();
        }

        var output = new List<QuestionnaireTemplateItemDto>();
        foreach (var row in rows)
        {
            if (!Guid.TryParse(row.QuestionKey[TemplatePrefix.Length..], out var id))
                continue;

            var payload = JsonSerializer.Deserialize<QuestionnaireTemplatePayload>(row.ResponseValue);
            if (payload is null || string.IsNullOrWhiteSpace(payload.QuestionText))
                continue;

            output.Add(new QuestionnaireTemplateItemDto(id, payload.QuestionText, payload.IsRequired, payload.SortOrder));
        }

        return output;
    }

    private sealed record QuestionnaireTemplatePayload(string QuestionText, bool IsRequired, int SortOrder);
}

[Authorize]
public class FilesController(ICsvTransformService csvTransformService) : BaseApiController
{
    [HttpPost("transform")]
    public async Task<ActionResult<ApiResponseDto<string>>> Transform([FromBody] string csv, CancellationToken ct)
    {
        var result = await csvTransformService.ValidateAndTransformAsync(csv, ct);
        if (!result.IsValid)
        {
            return BadRequest(new ApiResponseDto<string>(false, null, new ApiErrorDto("invalid_csv", string.Join(";", result.Errors))));
        }

        return Ok(new ApiResponseDto<string>(true, result.TransformedCsv));
    }
}

[Authorize]
public class CustomerSuccessController : BaseApiController
{
    [HttpGet("health")]
    public IActionResult Health() => Ok(new ApiResponseDto<string>(true, "Customer success operations ready"));
}

[Authorize(Roles = "WrkPlanAdmin")]
public class ReportsController(AdminDbContext dbContext) : BaseApiController
{
    [HttpPost("audit")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<AuditReportItemDto>>>> Audit([FromBody] AuditReportFilterDto request, CancellationToken ct)
    {
        var rows = await BuildAuditRowsAsync(request, ct);
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize > 250 ? 250 : request.PageSize;

        var sorted = ApplyAuditSort(rows, request.SortBy, request.SortOrder).ToList();
        var totalCount = sorted.Count;
        var page = sorted.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new ApiResponseDto<PagedResultDto<AuditReportItemDto>>(true,
            new PagedResultDto<AuditReportItemDto>(pageNumber, pageSize, totalCount, totalPages, page)));
    }

    [HttpPost("audit-export")]
    public async Task<IActionResult> AuditExport([FromBody] AuditReportFilterDto request, CancellationToken ct)
    {
        var rows = ApplyAuditSort(await BuildAuditRowsAsync(request with { PageSize = 5000, PageNumber = 1 }, ct), request.SortBy, request.SortOrder).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("OccurredUtc,EventType,Action,EntityType,EntityId,TenantName,User,UserRole,Details");
        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(',',
                Csv(row.OccurredUtc.ToString("O")),
                Csv(row.EventType),
                Csv(row.Action),
                Csv(row.EntityType),
                Csv(row.EntityId),
                Csv(row.TenantName ?? string.Empty),
                Csv(row.User),
                Csv(row.UserRole),
                Csv(row.Details)));
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"audit-report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }

    [HttpGet("cross-tenant")]
    public IActionResult CrossTenant() => Ok(new ApiResponseDto<string>(true, "Cross-tenant reporting ready"));

    private async Task<List<AuditReportItemDto>> BuildAuditRowsAsync(AuditReportFilterDto request, CancellationToken ct)
    {
        var tenants = await dbContext.TenantRegistries.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        var fromUtc = request.FromUtc ?? DateTime.UtcNow.AddMonths(-3);
        var toUtc = request.ToUtc ?? DateTime.UtcNow.AddDays(1);

        var rows = new List<AuditReportItemDto>();

        var logins = await dbContext.LoginAuditLogs.AsNoTracking()
            .Where(x => x.CreatedUtc >= fromUtc && x.CreatedUtc <= toUtc)
            .ToListAsync(ct);
        rows.AddRange(logins.Select(x => new AuditReportItemDto(
            x.CreatedUtc,
            "Login",
            x.Success ? "LoginSuccess" : "LoginFailed",
            "Auth",
            x.Id.ToString(),
            tenants.TryGetValue(x.TenantId, out var tenantName) ? tenantName : null,
            x.UserId.ToString(),
            "Unknown",
            $"IP={x.IpAddress}")));

        var payments = await dbContext.Payments.AsNoTracking()
            .Where(x => x.PaidUtc >= fromUtc && x.PaidUtc <= toUtc)
            .ToListAsync(ct);
        rows.AddRange(payments.Select(x => new AuditReportItemDto(
            x.PaidUtc,
            "Payment",
            x.Source == "Manual" ? "ManualPayment" : "OnlinePayment",
            "Payment",
            x.Id.ToString(),
            null,
            x.ProviderRef,
            "System",
            $"Mode={x.Mode};Amount={x.Amount};Status={x.Status}")));

        var announcements = await dbContext.AuditEvents.AsNoTracking()
            .Where(x => x.EventType == "Announcement" && x.OccurredUtc >= fromUtc && x.OccurredUtc <= toUtc)
            .ToListAsync(ct);
        rows.AddRange(announcements.Select(x => new AuditReportItemDto(
            x.OccurredUtc,
            "Announcement",
            x.Action,
            x.EntityType,
            x.EntityId,
            x.TenantId.HasValue && tenants.TryGetValue(x.TenantId.Value, out var tName) ? tName : null,
            x.UserEmail,
            x.UserRole,
            x.DetailsJson)));

        var ticketEvents = await dbContext.TicketStatusHistories.AsNoTracking()
            .Where(x => x.CreatedUtc >= fromUtc && x.CreatedUtc <= toUtc)
            .ToListAsync(ct);
        rows.AddRange(ticketEvents.Select(x => new AuditReportItemDto(
            x.CreatedUtc,
            "TicketStatus",
            $"{x.FromStatus}->{x.ToStatus}",
            "SupportTicket",
            x.SupportTicketId.ToString(),
            null,
            x.ChangedBy,
            x.ChangedByRole,
            x.Notes ?? string.Empty)));

        var contractEvents = await dbContext.ContractStatusHistories.AsNoTracking()
            .Where(x => x.CreatedUtc >= fromUtc && x.CreatedUtc <= toUtc)
            .ToListAsync(ct);
        rows.AddRange(contractEvents.Select(x => new AuditReportItemDto(
            x.CreatedUtc,
            "Contract",
            x.Action,
            "Contract",
            x.ContractId.ToString(),
            null,
            x.ActorEmail,
            x.ActorRole,
            x.Notes ?? string.Empty)));

        IEnumerable<AuditReportItemDto> output = rows;
        if (request.TenantId.HasValue)
            output = output.Where(x => string.Equals(x.TenantName, tenants.GetValueOrDefault(request.TenantId.Value), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(request.EventType))
            output = output.Where(x => x.EventType.Contains(request.EventType, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(request.User))
            output = output.Where(x => x.User.Contains(request.User, StringComparison.OrdinalIgnoreCase));

        return output.ToList();
    }

    private static IEnumerable<AuditReportItemDto> ApplyAuditSort(IEnumerable<AuditReportItemDto> source, string? sortBy, string? sortOrder)
    {
        var desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        return (sortBy ?? "occurredUtc").Trim().ToLowerInvariant() switch
        {
            "eventtype" => desc ? source.OrderByDescending(x => x.EventType) : source.OrderBy(x => x.EventType),
            "user" => desc ? source.OrderByDescending(x => x.User) : source.OrderBy(x => x.User),
            _ => desc ? source.OrderByDescending(x => x.OccurredUtc) : source.OrderBy(x => x.OccurredUtc)
        };
    }

    private static string Csv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}

[Authorize(Roles = "WrkPlanAdmin")]
public class AuditController(AdminDbContext dbContext) : BaseApiController
{
    [HttpGet("logins")]
    public async Task<ActionResult<ApiResponseDto<List<LoginAuditLog>>>> Logins(CancellationToken ct)
    {
        var rows = await dbContext.LoginAuditLogs.AsNoTracking().OrderByDescending(x => x.CreatedUtc).Take(200).ToListAsync(ct);
        return Ok(new ApiResponseDto<List<LoginAuditLog>>(true, rows));
    }
}

[Authorize(Roles = "WrkPlanAdmin")]
public class IntegrationsController : BaseApiController
{
    [HttpPost("email/send")]
    public IActionResult SendEmail() => Ok(new ApiResponseDto<string>(true, "Email integration abstraction endpoint ready"));

    [HttpPost("calendar/sync")]
    public IActionResult Calendar() => Ok(new ApiResponseDto<string>(true, "Calendar integration abstraction endpoint ready"));

    [HttpPost("esign/send")]
    public IActionResult ESign() => Ok(new ApiResponseDto<string>(true, "E-sign abstraction endpoint ready"));

    [HttpPost("jotform/hook")]
    [AllowAnonymous]
    public IActionResult JotFormHook() => Ok(new ApiResponseDto<string>(true, "JotForm hook endpoint ready"));
}

[Authorize(Roles = "CustomerAdmin")]
public class ThemeController(IThemePreferenceService themeService, TenantContext tenantContext, AdminDbContext dbContext) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<ThemePreferenceDto>>> Get(CancellationToken ct)
    {
        var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
        return Ok(new ApiResponseDto<ThemePreferenceDto>(true, await themeService.GetAsync(customerProfileId, ct)));
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponseDto<ThemePreferenceDto>>> Save([FromBody] ThemePreferenceDto request, CancellationToken ct)
    {
        var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
        return Ok(new ApiResponseDto<ThemePreferenceDto>(true, await themeService.SaveAsync(customerProfileId, request, ct)));
    }

    [HttpDelete("reset")]
    public async Task<IActionResult> Reset(CancellationToken ct)
    {
        var customerProfileId = await ResolveCustomerProfileIdAsync(ct);
        await themeService.ResetAsync(customerProfileId, ct);
        return Ok(new ApiResponseDto<string>(true, "Reset complete"));
    }

    private async Task<Guid> ResolveCustomerProfileIdAsync(CancellationToken ct)
    {
        var customerProfileId = await dbContext.CustomerProfiles
            .Where(x => x.TenantId == tenantContext.TenantId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        return customerProfileId == Guid.Empty ? tenantContext.TenantId : customerProfileId;
    }
}
