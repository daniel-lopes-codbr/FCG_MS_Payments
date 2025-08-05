using FCG_MS_Payments.Application.DTOs;

namespace FCG_MS_Payments.Application.Interfaces;

public interface IPaymentApplicationService
{
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
    Task<PaymentResponse> ConfirmPaymentAsync(ConfirmPaymentRequest request);
    Task<PaymentResponse> GetPaymentAsync(Guid id);
    Task<PaymentResponse> GetPaymentByStripeIdAsync(string stripePaymentIntentId);
    Task<IEnumerable<PaymentResponse>> GetPaymentsByCustomerAsync(string customerId);
} 