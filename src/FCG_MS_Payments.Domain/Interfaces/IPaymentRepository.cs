using FCG_MS_Payments.Domain.Entities;

namespace FCG_MS_Payments.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(string customerId);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
    Task<bool> DeleteAsync(Guid id);
} 