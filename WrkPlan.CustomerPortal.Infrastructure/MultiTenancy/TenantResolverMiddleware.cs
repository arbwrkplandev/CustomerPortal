using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using WrkPlan.CustomerPortal.Infrastructure.Data;

namespace WrkPlan.CustomerPortal.Infrastructure.MultiTenancy;

public class TenantResolverMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, AdminDbContext dbContext)
    {
        var claimTenantId = context.User.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(claimTenantId, out var parsedTenantId))
        {
            tenantContext.TenantId = parsedTenantId;
            tenantContext.TenantKey = context.User.FindFirst("tenant_key")?.Value ?? string.Empty;
            await next(context);
            return;
        }

        var tenantKey = context.Request.Headers["X-Tenant-Key"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(tenantKey))
        {
            var tenant = await dbContext.TenantRegistries.AsNoTracking().FirstOrDefaultAsync(t => t.TenantKey == tenantKey);
            if (tenant is not null)
            {
                tenantContext.TenantId = tenant.Id;
                tenantContext.TenantKey = tenant.TenantKey;
            }
        }

        await next(context);
    }
}
