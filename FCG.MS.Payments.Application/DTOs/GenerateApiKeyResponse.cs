namespace FCG.MS.Payments.Application.DTOs;

public class GenerateApiKeyResponse
{
    public string ApiKey { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

