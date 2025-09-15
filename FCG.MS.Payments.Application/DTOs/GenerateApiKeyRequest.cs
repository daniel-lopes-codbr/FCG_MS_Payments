namespace FCG.MS.Payments.Application.DTOs;

public class GenerateApiKeyRequest
{
    public string ServiceName { get; set; } = string.Empty;
    public string AdminSecret { get; set; } = string.Empty;
}

