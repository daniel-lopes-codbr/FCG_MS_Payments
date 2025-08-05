using FCG_MS_Payments.Domain.Entities;
using FCG_MS_Payments.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FCG_MS_Payments.Infrastructure.Repositories;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly Dictionary<Guid, Payment> _payments = new();
    private readonly Dictionary<string, Payment> _paymentsByStripeId = new();
    private readonly ILogger<InMemoryPaymentRepository> _logger;

    public InMemoryPaymentRepository(ILogger<InMemoryPaymentRepository> logger)
    {
        _logger = logger;
    }

    public Task<Payment?> GetByIdAsync(Guid id)
    {
        _payments.TryGetValue(id, out var payment);
        return Task.FromResult(payment);
    }

    public Task<Payment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId)
    {
        _paymentsByStripeId.TryGetValue(stripePaymentIntentId, out var payment);
        return Task.FromResult(payment);
    }

    public Task<IEnumerable<Payment>> GetByCustomerIdAsync(string customerId)
    {
        var payments = _payments.Values.Where(p => p.CustomerId == customerId).ToList();
        return Task.FromResult<IEnumerable<Payment>>(payments);
    }

    public Task<Payment> CreateAsync(Payment payment)
    {
        _payments[payment.Id] = payment;
        if (!string.IsNullOrEmpty(payment.StripePaymentIntentId))
        {
            _paymentsByStripeId[payment.StripePaymentIntentId] = payment;
        }
        
        _logger.LogInformation("Created payment with ID: {PaymentId}", payment.Id);
        return Task.FromResult(payment);
    }

    public Task<Payment> UpdateAsync(Payment payment)
    {
        if (_payments.ContainsKey(payment.Id))
        {
            _payments[payment.Id] = payment;
            if (!string.IsNullOrEmpty(payment.StripePaymentIntentId))
            {
                _paymentsByStripeId[payment.StripePaymentIntentId] = payment;
            }
            
            _logger.LogInformation("Updated payment with ID: {PaymentId}", payment.Id);
            return Task.FromResult(payment);
        }

        throw new InvalidOperationException($"Payment with ID {payment.Id} not found");
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        if (_payments.TryGetValue(id, out var payment))
        {
            _payments.Remove(id);
            if (!string.IsNullOrEmpty(payment.StripePaymentIntentId))
            {
                _paymentsByStripeId.Remove(payment.StripePaymentIntentId);
            }
            
            _logger.LogInformation("Deleted payment with ID: {PaymentId}", id);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
} 