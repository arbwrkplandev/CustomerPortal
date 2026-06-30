using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.Application.Interfaces;

public interface IJwtTokenService
{
    AuthResponseDto CreateToken(Guid tenantId, Guid userId, string email, string role);
}

public interface ITenantProvisioningService
{
    Task<TenantProvisionResponseDto> ProvisionTenantAsync(TenantProvisionRequestDto request, CancellationToken ct = default);
    Task ActivateTenantAfterFirstPaymentAsync(Guid tenantId, CancellationToken ct = default);
}

public interface IThemePreferenceService
{
    Task<ThemePreferenceDto> GetAsync(Guid customerProfileId, CancellationToken ct = default);
    Task<ThemePreferenceDto> SaveAsync(Guid customerProfileId, ThemePreferenceDto request, CancellationToken ct = default);
    Task ResetAsync(Guid customerProfileId, CancellationToken ct = default);
}

public interface IDashboardService
{
    Task<DashboardResponseDto> GetCustomerHubDashboardAsync(CancellationToken ct = default);
    Task<DashboardResponseDto> GetCustomerPortalDashboardAsync(Guid customerProfileId, CancellationToken ct = default);
}

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(Guid customerProfileId, decimal amount, bool isSubscription, string cycle, CancellationToken ct = default);
    Task ProcessWebhookAsync(string json, string signatureHeader, CancellationToken ct = default);
}

public interface IOnboardingEngineService
{
    bool CanCompleteStep(IReadOnlyCollection<OnboardingStepDto> steps, Guid stepId);
    decimal CalculateProgressPercent(IReadOnlyCollection<OnboardingStepDto> steps);
}

public interface ICsvTransformService
{
    Task<(bool IsValid, List<string> Errors, string TransformedCsv)> ValidateAndTransformAsync(string csvPayload, CancellationToken ct = default);
}

public interface IRenewalService
{
    DateTime CalculateNextRenewal(DateTime startDateUtc, string cycle);
}

public interface IQuoteTransitionService
{
    (DateTime StartDateUtc, DateTime EndDateUtc, DateTime RenewalDateUtc) BuildSubscriptionTimeline(DateTime acceptedAtUtc, string cycle);
}

