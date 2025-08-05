using FCG_MS_Payments.Domain.Enums;

namespace FCG_MS_Payments.Domain.Interfaces;

public interface IStripeService
{
    Task<StripePaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, string customerId, string description, Dictionary<string, string>? metadata = null);
    Task<StripePaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId);
    Task<StripePaymentIntent> GetPaymentIntentAsync(string paymentIntentId);
    PaymentStatus MapStripeStatusToPaymentStatus(string stripeStatus);
}

public class StripePaymentIntent
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
} 