using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FCG.MS.Payments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public CheckoutController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Creates a payment intent for checkout
    /// </summary>
    /// <param name="request">Checkout request with customer and product information</param>
    /// <returns>Payment intent with client secret for frontend processing</returns>
    [HttpPost]
    public async Task<ActionResult<CreateCheckoutResponse>> CreateCheckout([FromBody] CreateCheckoutRequest request)
    {
        try
        {
            Console.WriteLine($"🔍 Checkout request received: CustomerId={request.CustomerId}, ProductId={request.ProductId}, Quantity={request.Quantity}");
            
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.CustomerId))
            {
                Console.WriteLine("❌ Validation failed: CustomerId is required");
                return BadRequest(new { error = "CustomerId is required" });
            }

            if (string.IsNullOrWhiteSpace(request.ProductId))
            {
                Console.WriteLine("❌ Validation failed: ProductId is required");
                return BadRequest(new { error = "ProductId is required" });
            }

            if (request.Quantity <= 0)
            {
                Console.WriteLine("❌ Validation failed: Quantity must be greater than zero");
                return BadRequest(new { error = "Quantity must be greater than zero" });
            }

            Console.WriteLine("✅ Validation passed, calling CreateCheckoutAsync...");
            var response = await _paymentService.CreateCheckoutAsync(request);
            Console.WriteLine($"✅ Checkout session created successfully: SessionId={response.SessionId}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Checkout error: {ex.Message}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the status of a payment intent
    /// </summary>
    /// <param name="paymentIntentId">Payment intent ID to check</param>
    /// <returns>Payment status information</returns>
    [HttpGet("payments/{paymentIntentId}")]
    public async Task<ActionResult<PaymentStatusResponse>> GetPaymentStatus(string paymentIntentId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId))
            {
                return BadRequest(new { error = "PaymentIntentId is required" });
            }

            var paymentStatus = await _paymentService.GetPaymentStatusAsync(paymentIntentId);
            return Ok(paymentStatus);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
