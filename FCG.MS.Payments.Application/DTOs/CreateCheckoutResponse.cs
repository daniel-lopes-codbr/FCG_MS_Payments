namespace FCG.MS.Payments.Application.DTOs;

public class CreateCheckoutResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;  // externalCustomerId
    public string ProductId { get; set; } = string.Empty;   // externalProductId
    public int Quantity { get; set; }
}
