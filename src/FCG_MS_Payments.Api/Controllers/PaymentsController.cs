using Microsoft.AspNetCore.Mvc;
using FCG_MS_Payments.Application.DTOs;
using FCG_MS_Payments.Application.Interfaces;
using FCG_MS_Payments.Api.Models;
using FCG_MS_Payments.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace FCG_MS_Payments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentApplicationService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentApplicationService paymentService,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new payment
    /// </summary>
    /// <param name="request">Payment creation request</param>
    /// <returns>Created payment information</returns>
    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(request);
            var response = ApiResponse<PaymentResponse>.SuccessResponse(payment, "Payment created successfully");
            
            _logger.LogInformation("Payment created successfully: {PaymentId}", payment.Id);
            return Ok(response);
        }
        catch (PaymentException ex)
        {
            _logger.LogWarning(ex, "Payment creation failed");
            var response = ApiResponse<PaymentResponse>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during payment creation");
            var response = ApiResponse<PaymentResponse>.ErrorResponse("An unexpected error occurred");
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Confirms a payment
    /// </summary>
    /// <param name="request">Payment confirmation request</param>
    /// <returns>Updated payment information</returns>
    [HttpPost("confirm")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        try
        {
            var payment = await _paymentService.ConfirmPaymentAsync(request);
            var response = ApiResponse<PaymentResponse>.SuccessResponse(payment, "Payment confirmed successfully");
            
            _logger.LogInformation("Payment confirmed successfully: {PaymentId}", payment.Id);
            return Ok(response);
        }
        catch (PaymentException ex)
        {
            _logger.LogWarning(ex, "Payment confirmation failed");
            var response = ApiResponse<PaymentResponse>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during payment confirmation");
            var response = ApiResponse<PaymentResponse>.ErrorResponse("An unexpected error occurred");
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Gets a payment by ID
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>Payment information</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPayment(Guid id)
    {
        try
        {
            var payment = await _paymentService.GetPaymentAsync(id);
            var response = ApiResponse<PaymentResponse>.SuccessResponse(payment);
            
            return Ok(response);
        }
        catch (PaymentException ex)
        {
            _logger.LogWarning(ex, "Payment retrieval failed for ID: {PaymentId}", id);
            var response = ApiResponse<PaymentResponse>.ErrorResponse(ex.Message);
            return NotFound(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving payment: {PaymentId}", id);
            var response = ApiResponse<PaymentResponse>.ErrorResponse("An unexpected error occurred");
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Gets a payment by Stripe payment intent ID
    /// </summary>
    /// <param name="stripePaymentIntentId">Stripe payment intent ID</param>
    /// <returns>Payment information</returns>
    [HttpGet("stripe/{stripePaymentIntentId}")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPaymentByStripeId(string stripePaymentIntentId)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByStripeIdAsync(stripePaymentIntentId);
            var response = ApiResponse<PaymentResponse>.SuccessResponse(payment);
            
            return Ok(response);
        }
        catch (PaymentException ex)
        {
            _logger.LogWarning(ex, "Payment retrieval failed for Stripe ID: {StripeId}", stripePaymentIntentId);
            var response = ApiResponse<PaymentResponse>.ErrorResponse(ex.Message);
            return NotFound(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving payment: {StripeId}", stripePaymentIntentId);
            var response = ApiResponse<PaymentResponse>.ErrorResponse("An unexpected error occurred");
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Gets all payments for a customer
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <returns>List of payments</returns>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponse>>>> GetPaymentsByCustomer(string customerId)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByCustomerAsync(customerId);
            var response = ApiResponse<IEnumerable<PaymentResponse>>.SuccessResponse(payments);
            
            return Ok(response);
        }
        catch (PaymentException ex)
        {
            _logger.LogWarning(ex, "Payment retrieval failed for customer: {CustomerId}", customerId);
            var response = ApiResponse<IEnumerable<PaymentResponse>>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving payments for customer: {CustomerId}", customerId);
            var response = ApiResponse<IEnumerable<PaymentResponse>>.ErrorResponse("An unexpected error occurred");
            return StatusCode(500, response);
        }
    }
} 