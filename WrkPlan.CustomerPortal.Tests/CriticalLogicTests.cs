using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using System.Text;
using Stripe;
using WrkPlan.CustomerPortal.Application.Interfaces;
using WrkPlan.CustomerPortal.Application.Services;
using WrkPlan.CustomerPortal.Domain.Entities;
using WrkPlan.CustomerPortal.Infrastructure.Data;
using WrkPlan.CustomerPortal.Infrastructure.Services;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.Tests;

public class CriticalLogicTests
{
    [Theory]
    [InlineData("monthly", 1)]
    [InlineData("quarterly", 3)]
    [InlineData("annual", 12)]
    public void RenewalCycle_GeneratesExpectedMonths(string cycle, int expectedMonths)
    {
        var service = new RenewalService();
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var next = service.CalculateNextRenewal(start, cycle);

        Assert.Equal(start.AddMonths(expectedMonths), next);
    }

    [Fact]
    public void QuoteToSubscription_BuildsTimeline()
    {
        var renewal = new RenewalService();
        var service = new QuoteTransitionService(renewal);

        var accepted = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var timeline = service.BuildSubscriptionTimeline(accepted, "monthly");

        Assert.Equal(accepted, timeline.StartDateUtc);
        Assert.Equal(accepted.AddMonths(1), timeline.RenewalDateUtc);
        Assert.Equal(accepted.AddYears(1), timeline.EndDateUtc);
    }

    [Fact]
    public async Task FirstPaymentActivation_ActivatesTenantAndPortal()
    {
        var options = new DbContextOptionsBuilder<AdminDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new AdminDbContext(options);
        var tenantId = Guid.NewGuid();
        db.TenantRegistries.Add(new TenantRegistry { Id = tenantId, TenantKey = "acme", Name = "Acme", IsActive = false });
        db.CustomerProfiles.Add(new CustomerProfile { Id = Guid.NewGuid(), TenantId = tenantId, CompanyName = "Acme", IsPortalActive = false });
        await db.SaveChangesAsync();

        var service = new TenantProvisioningService(db, NullLogger<TenantProvisioningService>.Instance);
        await service.ActivateTenantAfterFirstPaymentAsync(tenantId);

        var tenant = await db.TenantRegistries.SingleAsync();
        var profile = await db.CustomerProfiles.SingleAsync();
        Assert.True(tenant.IsActive);
        Assert.True(profile.IsPortalActive);
    }

    [Fact]
    public async Task StripeWebhook_ValidatesSignature_AndIdempotency()
    {
        var options = new DbContextOptionsBuilder<AdminDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new AdminDbContext(options);
        var tenantId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        db.TenantRegistries.Add(new TenantRegistry { Id = tenantId, TenantKey = "northwind", Name = "Northwind", IsActive = false });
        db.CustomerProfiles.Add(new CustomerProfile { Id = profileId, TenantId = tenantId, CompanyName = "Northwind", IsPortalActive = false });
        await db.SaveChangesAsync();

        var secret = "whsec_test_secret";
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Stripe:WebhookSecret"] = secret,
            ["Stripe:SecretKey"] = "sk_test"
        }).Build();

        var fakeProvision = new FakeTenantProvisioningService(db);
        var service = new StripePaymentService(config, db, fakeProvision, NullLogger<StripePaymentService>.Instance);

        var json = $"{{\"id\":\"evt_1\",\"object\":\"event\",\"type\":\"checkout.session.completed\",\"data\":{{\"object\":{{\"id\":\"cs_123\",\"object\":\"checkout.session\",\"metadata\":{{\"customerProfileId\":\"{profileId}\"}}}}}}}}";
        var signature = BuildStripeSignatureHeader(json, secret);

        await service.ProcessWebhookAsync(json, signature);
        await service.ProcessWebhookAsync(json, signature);

        Assert.Equal(1, await db.PaymentEvents.CountAsync());
        Assert.Equal(1, fakeProvision.ActivationCalls);
    }

    [Fact]
    public void OnboardingEngine_RespectsDependency()
    {
        var engine = new OnboardingEngineService();
        var depId = Guid.NewGuid();
        var targetId = Guid.NewGuid();

        var steps = new List<OnboardingStepDto>
        {
            new(depId, "Setup", true, false, null),
            new(targetId, "Questionnaire", true, false, depId)
        };

        Assert.False(engine.CanCompleteStep(steps, targetId));

        steps[0] = steps[0] with { IsCompleted = true };

        Assert.True(engine.CanCompleteStep(steps, targetId));
    }

    [Fact]
    public async Task CsvTransform_ValidatesAndTransforms()
    {
        var service = new CsvTransformService();
        var csv = "EmployeeId,EmployeeName,Department\n1,Jane,Finance";

        var result = await service.ValidateAndTransformAsync(csv);

        Assert.True(result.IsValid);
        Assert.Contains("ERPEmployeeCode", result.TransformedCsv);
    }

    private sealed class FakeTenantProvisioningService(AdminDbContext db) : ITenantProvisioningService
    {
        public int ActivationCalls { get; private set; }

        public Task<TenantProvisionResponseDto> ProvisionTenantAsync(TenantProvisionRequestDto request, CancellationToken ct = default)
            => Task.FromResult(new TenantProvisionResponseDto(Guid.NewGuid(), "db", request.AdminEmail, "pass"));

        public async Task ActivateTenantAfterFirstPaymentAsync(Guid tenantId, CancellationToken ct = default)
        {
            ActivationCalls++;
            var tenant = await db.TenantRegistries.SingleAsync(x => x.Id == tenantId, ct);
            tenant.IsActive = true;
            await db.SaveChangesAsync(ct);
        }
    }

    private static string BuildStripeSignatureHeader(string payload, string secret)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var signedPayload = $"{timestamp}.{payload}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload));
        var signature = Convert.ToHexString(hash).ToLowerInvariant();

        return $"t={timestamp},v1={signature}";
    }
}
