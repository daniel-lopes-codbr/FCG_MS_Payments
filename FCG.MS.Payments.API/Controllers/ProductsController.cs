using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FCG.MS.Payments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public ProductsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Gets all products from Stripe
    /// </summary>
    /// <returns>List of all products</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductListResponse>>> GetAllProducts()
    {
        try
        {
            var products = await _paymentService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets a product by external ID
    /// </summary>
    /// <param name="externalProductId">External product ID to search for</param>
    /// <returns>Product information if found</returns>
    [HttpGet("{externalProductId}")]
    public async Task<ActionResult<ProductListResponse>> GetProductByExternalId(string externalProductId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(externalProductId))
            {
                return BadRequest(new { error = "ExternalProductId is required" });
            }

            var product = await _paymentService.GetProductByExternalIdAsync(externalProductId);
            if (product == null)
            {
                return NotFound(new { error = $"Product with external ID '{externalProductId}' not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new product in Stripe
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <returns>Created product information</returns>
    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.ExternalProductId))
            {
                return BadRequest(new { error = "ExternalProductId is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { error = "Name is required" });
            }

            if (request.Price <= 0)
            {
                return BadRequest(new { error = "Price must be greater than zero" });
            }

            // Validate ImageUrl if provided
            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                if (!Uri.TryCreate(request.ImageUrl, UriKind.Absolute, out _))
                {
                    return BadRequest(new { error = "ImageUrl must be a valid absolute URL" });
                }
            }

            var response = await _paymentService.CreateProductAsync(request);
            return CreatedAtAction(nameof(CreateProduct), response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
