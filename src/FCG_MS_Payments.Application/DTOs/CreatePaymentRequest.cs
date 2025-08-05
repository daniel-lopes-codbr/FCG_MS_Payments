namespace FCG_MS_Payments.Application.DTOs;

public class CreatePaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    public string CustomerId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string>? Metadata { get; set; }
} 