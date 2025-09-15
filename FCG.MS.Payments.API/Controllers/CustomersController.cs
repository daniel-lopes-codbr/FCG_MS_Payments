using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FCG.MS.Payments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public CustomersController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Creates a new customer in Stripe
    /// </summary>
    /// <param name="request">Customer creation request</param>
    /// <returns>Created customer information</returns>
    [HttpPost]
    public async Task<ActionResult<CreateCustomerResponse>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.ExternalCustomerId))
            {
                return BadRequest(new { error = "ExternalCustomerId is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { error = "Name is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { error = "Email is required" });
            }

            // Validate email format
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { error = "Email must be a valid email address" });
            }

            // Validate phone format if provided
            if (!string.IsNullOrWhiteSpace(request.Phone) && !IsValidPhone(request.Phone))
            {
                return BadRequest(new { error = "Phone must be a valid phone number" });
            }

            var response = await _paymentService.CreateCustomerAsync(request);
            return CreatedAtAction(nameof(CreateCustomer), response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets a customer by external customer ID
    /// </summary>
    /// <param name="externalCustomerId">External customer ID to search for</param>
    /// <returns>Customer information if found</returns>
    [HttpGet("{externalCustomerId}")]
    public async Task<ActionResult<CustomerListResponse>> GetCustomer(string externalCustomerId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(externalCustomerId))
            {
                return BadRequest(new { error = "ExternalCustomerId is required" });
            }

            var customer = await _paymentService.GetCustomerAsync(externalCustomerId);
            
            if (customer == null)
            {
                return NotFound(new { error = "Customer not found" });
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhone(string phone)
    {
        // Basic phone validation - allows digits, spaces, hyphens, parentheses, and plus sign
        return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[\+]?[0-9\s\-\(\)]+$");
    }
}
