using AutoMapper;
using FCG_MS_Payments.Application.DTOs;
using FCG_MS_Payments.Application.Interfaces;
using FCG_MS_Payments.Domain.Interfaces;
using FCG_MS_Payments.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FCG_MS_Payments.Application.Services;

public class PaymentApplicationService : IPaymentApplicationService
{
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentApplicationService> _logger;

    public PaymentApplicationService(
        IPaymentService paymentService,
        IMapper mapper,
        ILogger<PaymentApplicationService> logger)
    {
        _paymentService = paymentService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(
                request.Amount,
                request.Currency,
                request.CustomerId,
                request.Description,
                request.Metadata);

            var response = _mapper.Map<PaymentResponse>(payment);
            
            // Add client secret for frontend
            response.ClientSecret = payment.StripePaymentIntentId; // In a real implementation, you'd get this from Stripe

            _logger.LogInformation("Created payment response for ID: {PaymentId}", payment.Id);
            return response;
        }
        catch (PaymentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            throw new PaymentException("Failed to create payment", ex);
        }
    }

    public async Task<PaymentResponse> ConfirmPaymentAsync(ConfirmPaymentRequest request)
    {
        try
        {
            var payment = await _paymentService.ConfirmPaymentAsync(request.StripePaymentIntentId);
            var response = _mapper.Map<PaymentResponse>(payment);

            _logger.LogInformation("Confirmed payment response for ID: {PaymentId}", payment.Id);
            return response;
        }
        catch (PaymentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming payment");
            throw new PaymentException("Failed to confirm payment", ex);
        }
    }

    public async Task<PaymentResponse> GetPaymentAsync(Guid id)
    {
        try
        {
            var payment = await _paymentService.GetPaymentAsync(id);
            var response = _mapper.Map<PaymentResponse>(payment);

            return response;
        }
        catch (PaymentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with ID: {PaymentId}", id);
            throw new PaymentException("Failed to retrieve payment", ex);
        }
    }

    public async Task<PaymentResponse> GetPaymentByStripeIdAsync(string stripePaymentIntentId)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByStripeIdAsync(stripePaymentIntentId);
            var response = _mapper.Map<PaymentResponse>(payment);

            return response;
        }
        catch (PaymentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with Stripe ID: {StripeId}", stripePaymentIntentId);
            throw new PaymentException("Failed to retrieve payment", ex);
        }
    }

    public async Task<IEnumerable<PaymentResponse>> GetPaymentsByCustomerAsync(string customerId)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByCustomerAsync(customerId);
            var responses = _mapper.Map<IEnumerable<PaymentResponse>>(payments);

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for customer: {CustomerId}", customerId);
            throw new PaymentException("Failed to retrieve payments", ex);
        }
    }
} 