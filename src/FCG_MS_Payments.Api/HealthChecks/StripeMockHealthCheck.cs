using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using FCG_MS_Payments.Infrastructure.Models;

namespace FCG_MS_Payments.Api.HealthChecks;

public class StripeMockHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly StripeSettings _stripeSettings;

    public StripeMockHealthCheck(HttpClient httpClient, IOptions<StripeSettings> stripeSettings)
    {
        _httpClient = httpClient;
        _stripeSettings = stripeSettings.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!_stripeSettings.UseMockServer)
        {
            return HealthCheckResult.Healthy("Mock server not enabled");
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_stripeSettings.MockServerUrl}/health", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Stripe mock server is responding");
            }
            else
            {
                return HealthCheckResult.Degraded($"Stripe mock server returned status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Stripe mock server is not accessible", ex);
        }
    }
} 