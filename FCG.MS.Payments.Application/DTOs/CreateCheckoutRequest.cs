namespace FCG.MS.Payments.Application.DTOs;

public class CreateCheckoutRequest
{
    public string CustomerId { get; set; } = string.Empty;  // externalCustomerId
    public string ProductId { get; set; } = string.Empty;   // externalProductId
    public int Quantity { get; set; } = 1;
}
