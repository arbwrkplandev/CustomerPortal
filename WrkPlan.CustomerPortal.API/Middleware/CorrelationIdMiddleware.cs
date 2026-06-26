namespace WrkPlan.CustomerPortal.API.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;
        await next(context);
    }
}
