using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FCG.MS.Payments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Creates a payment intent for a specific product
    /// </summary>
    /// <param name="request">Payment creation request</param>
    /// <returns>Payment intent with client secret</returns>
    [HttpPost("create")]
    public async Task<ActionResult<CreatePaymentResponse>> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var response = await _paymentService.CreatePaymentIntentAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the status of a payment by its ID
    /// </summary>
    /// <param name="id">Payment intent ID</param>
    /// <returns>Payment status information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentStatusResponse>> GetPaymentStatus(string id)
    {
        try
        {
            var response = await _paymentService.GetPaymentStatusAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
