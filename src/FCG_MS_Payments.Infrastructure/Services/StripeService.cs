using Stripe;
using FCG_MS_Payments.Domain.Entities;
using FCG_MS_Payments.Domain.Enums;
using FCG_MS_Payments.Domain.Interfaces;
using FCG_MS_Payments.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FCG_MS_Payments.Infrastructure.Models;

namespace FCG_MS_Payments.Infrastructure.Services;

public class StripeService : IStripeService
{
    private readonly ILogger<StripeService> _logger;
    private readonly StripeSettings _stripeSettings;

    public StripeService(ILogger<StripeService> logger, IOptions<StripeSettings> stripeSettings)
    {
        _logger = logger;
        _stripeSettings = stripeSettings.Value;
        
        // Configure Stripe
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        
        if (_stripeSettings.UseMockServer)
        {
            _logger.LogInformation("Using Stripe mock server at: {MockServerUrl}", _stripeSettings.MockServerUrl);
        }
        else
        {
            _logger.LogInformation("Using real Stripe API");
        }
    }

    public async Task<StripePaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, string customerId, string description, Dictionary<string, string>? metadata = null)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Convert to cents
                Currency = currency.ToLower(),
                Customer = customerId,
                Description = description,
                Metadata = metadata?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var domainPaymentIntent = new StripePaymentIntent
            {
                Id = paymentIntent.Id,
                Status = paymentIntent.Status,
                ClientSecret = paymentIntent.ClientSecret
            };

            _logger.LogInformation("Created Stripe payment intent: {PaymentIntentId} using {ServerType}", 
                paymentIntent.Id, _stripeSettings.UseMockServer ? "mock server" : "real Stripe");
            return domainPaymentIntent;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error creating Stripe payment intent using {ServerType}", 
                _stripeSettings.UseMockServer ? "mock server" : "real Stripe");
            throw new PaymentException($"Failed to create payment intent: {ex.Message}", ex);
        }
    }

    public async Task<StripePaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(paymentIntentId);

            var domainPaymentIntent = new StripePaymentIntent
            {
                Id = paymentIntent.Id,
                Status = paymentIntent.Status,
                ClientSecret = paymentIntent.ClientSecret
            };

            _logger.LogInformation("Confirmed Stripe payment intent: {PaymentIntentId} using {ServerType}", 
                paymentIntent.Id, _stripeSettings.UseMockServer ? "mock server" : "real Stripe");
            return domainPaymentIntent;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error confirming Stripe payment intent: {PaymentIntentId} using {ServerType}", 
                paymentIntentId, _stripeSettings.UseMockServer ? "mock server" : "real Stripe");
            throw new PaymentException($"Failed to confirm payment intent: {ex.Message}", ex);
        }
    }

    public async Task<StripePaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            var domainPaymentIntent = new StripePaymentIntent
            {
                Id = paymentIntent.Id,
                Status = paymentIntent.Status,
                ClientSecret = paymentIntent.ClientSecret
            };

            _logger.LogInformation("Retrieved Stripe payment intent: {PaymentIntentId} using {ServerType}", 
                paymentIntent.Id, _stripeSettings.UseMockServer ? "mock server" : "real Stripe");
            return domainPaymentIntent;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error retrieving Stripe payment intent: {PaymentIntentId} using {ServerType}", 
                paymentIntentId, _stripeSettings.UseMockServer ? "mock server" : "real Stripe");
            throw new PaymentException($"Failed to retrieve payment intent: {ex.Message}", ex);
        }
    }

    public PaymentStatus MapStripeStatusToPaymentStatus(string stripeStatus)
    {
        return stripeStatus.ToLower() switch
        {
            "requires_payment_method" => PaymentStatus.Pending,
            "requires_confirmation" => PaymentStatus.Pending,
            "requires_action" => PaymentStatus.Processing,
            "processing" => PaymentStatus.Processing,
            "succeeded" => PaymentStatus.Succeeded,
            "canceled" => PaymentStatus.Cancelled,
            _ => PaymentStatus.Failed
        };
    }
} 