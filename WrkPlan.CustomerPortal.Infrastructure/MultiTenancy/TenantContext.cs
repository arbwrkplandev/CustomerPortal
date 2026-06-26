namespace WrkPlan.CustomerPortal.Infrastructure.MultiTenancy;

public sealed class TenantContext
{
    public Guid TenantId { get; set; }
    public string TenantKey { get; set; } = string.Empty;
}
