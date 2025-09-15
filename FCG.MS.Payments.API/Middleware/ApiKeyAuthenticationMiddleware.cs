using Microsoft.Extensions.Options;
using FCG.MS.Payments.Infrastructure.Configuration;

namespace FCG.MS.Payments.API.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<ApiKeySettings> _apiKeySettings;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, IOptions<ApiKeySettings> apiKeySettings)
    {
        _next = next;
        _apiKeySettings = apiKeySettings;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for Swagger and Admin endpoints
        if (context.Request.Path.StartsWithSegments("/swagger") || 
            context.Request.Path.StartsWithSegments("/api/admin"))
        {
            await _next(context);
            return;
        }

        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "API key is required" });
            return;
        }

        if (apiKey != _apiKeySettings.Value.ApiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API key" });
            return;
        }

        await _next(context);
    }
}

public static class ApiKeyAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
    }
}
