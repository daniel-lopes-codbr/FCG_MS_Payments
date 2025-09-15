using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FCG.MS.Payments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public AdminController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    /// <summary>
    /// Generate a new API key for a service (Admin only)
    /// </summary>
    /// <param name="request">API key generation request</param>
    /// <returns>Generated API key information</returns>
    [HttpPost("generate-api-key")]
    public async Task<ActionResult<GenerateApiKeyResponse>> GenerateApiKey([FromBody] GenerateApiKeyRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.ServiceName))
            {
                return BadRequest(new { error = "ServiceName is required" });
            }

            if (string.IsNullOrWhiteSpace(request.AdminSecret))
            {
                return BadRequest(new { error = "AdminSecret is required" });
            }

            var response = await _apiKeyService.GenerateApiKeyAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

