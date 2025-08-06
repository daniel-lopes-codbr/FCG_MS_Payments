namespace FCG_MS_Payments.Infrastructure.Models;

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string MockServerUrl { get; set; } = "http://localhost:12111";
    public bool UseMockServer { get; set; } = false;
} 