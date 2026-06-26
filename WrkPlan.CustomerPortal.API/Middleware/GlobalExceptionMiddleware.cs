using System.Net;
using System.Text.Json;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.API.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
            var payload = new ApiResponseDto<object>(false, null, new ApiErrorDto("server_error", "An unexpected error occurred.", correlationId));
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
