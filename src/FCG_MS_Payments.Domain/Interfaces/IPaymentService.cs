using FCG_MS_Payments.Domain.Entities;

namespace FCG_MS_Payments.Domain.Interfaces;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(decimal amount, string currency, string customerId, string description, Dictionary<string, string>? metadata = null);
    Task<Payment> ConfirmPaymentAsync(string stripePaymentIntentId);
    Task<Payment> GetPaymentAsync(Guid id);
    Task<Payment> GetPaymentByStripeIdAsync(string stripePaymentIntentId);
    Task<IEnumerable<Payment>> GetPaymentsByCustomerAsync(string customerId);
} 