namespace FCG_MS_Payments.Application.DTOs;

public class ConfirmPaymentRequest
{
    public string StripePaymentIntentId { get; set; } = string.Empty;
} 