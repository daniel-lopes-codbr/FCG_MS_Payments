using FCG_MS_Payments.Domain.Entities;
using FCG_MS_Payments.Domain.Interfaces;
using FCG_MS_Payments.Domain.Enums;
using FCG_MS_Payments.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FCG_MS_Payments.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IStripeService _stripeService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IStripeService stripeService,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _stripeService = stripeService;
        _logger = logger;
    }

    public async Task<Payment> CreatePaymentAsync(decimal amount, string currency, string customerId, string description, Dictionary<string, string>? metadata = null)
    {
        try
        {
            // Create payment intent in Stripe
            var paymentIntent = await _stripeService.CreatePaymentIntentAsync(amount, currency, customerId, description, metadata);

            // Create payment entity
            var payment = new Payment
            {
                StripePaymentIntentId = paymentIntent.Id,
                Amount = amount,
                Currency = currency,
                CustomerId = customerId,
                Description = description,
                Status = _stripeService.MapStripeStatusToPaymentStatus(paymentIntent.Status),
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            // Save to repository
            var savedPayment = await _paymentRepository.CreateAsync(payment);

            _logger.LogInformation("Created payment: {PaymentId} with Stripe intent: {StripeIntentId}", 
                savedPayment.Id, savedPayment.StripePaymentIntentId);

            return savedPayment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for customer: {CustomerId}", customerId);
            throw new PaymentException("Failed to create payment", ex);
        }
    }

    public async Task<Payment> ConfirmPaymentAsync(string stripePaymentIntentId)
    {
        try
        {
            // Get payment from repository
            var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(stripePaymentIntentId);
            if (payment == null)
            {
                throw new PaymentException($"Payment not found for Stripe intent: {stripePaymentIntentId}");
            }

            // Confirm payment in Stripe
            var paymentIntent = await _stripeService.ConfirmPaymentIntentAsync(stripePaymentIntentId);

            // Update payment status
            var newStatus = _stripeService.MapStripeStatusToPaymentStatus(paymentIntent.Status);
            payment.UpdateStatus(newStatus);

            // Save updated payment
            var updatedPayment = await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation("Confirmed payment: {PaymentId} with status: {Status}", 
                updatedPayment.Id, updatedPayment.Status);

            return updatedPayment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming payment: {StripeIntentId}", stripePaymentIntentId);
            throw new PaymentException("Failed to confirm payment", ex);
        }
    }

    public async Task<Payment> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
        {
            throw new PaymentException($"Payment not found with ID: {id}");
        }

        return payment;
    }

    public async Task<Payment> GetPaymentByStripeIdAsync(string stripePaymentIntentId)
    {
        var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(stripePaymentIntentId);
        if (payment == null)
        {
            throw new PaymentException($"Payment not found with Stripe ID: {stripePaymentIntentId}");
        }

        return payment;
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByCustomerAsync(string customerId)
    {
        return await _paymentRepository.GetByCustomerIdAsync(customerId);
    }
} 