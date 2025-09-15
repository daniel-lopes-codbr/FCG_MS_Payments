using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using FCG.MS.Payments.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace FCG.MS.Payments.Infrastructure.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly ApiKeySettings _apiKeySettings;
    private readonly AdminSettings _adminSettings;

    public ApiKeyService(IOptions<ApiKeySettings> apiKeySettings, IOptions<AdminSettings> adminSettings)
    {
        _apiKeySettings = apiKeySettings.Value;
        _adminSettings = adminSettings.Value;
    }

    public Task<GenerateApiKeyResponse> GenerateApiKeyAsync(GenerateApiKeyRequest request)
    {
        // Validate admin secret
        if (request.AdminSecret != _adminSettings.AdminSecret)
        {
            throw new UnauthorizedAccessException("Invalid admin secret");
        }

        if (string.IsNullOrWhiteSpace(request.ServiceName))
        {
            throw new ArgumentException("Service name is required");
        }

        // Generate a secure API key
        var apiKey = GenerateSecureApiKey();
        var expiresAt = DateTime.UtcNow.AddDays(365); // 1 year expiration

        // In a real implementation, you would store this in a database
        // For this MVP, we'll just return the generated key
        // Note: In production, you should implement proper key storage and rotation

        var response = new GenerateApiKeyResponse
        {
            ApiKey = apiKey,
            ServiceName = request.ServiceName,
            GeneratedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        return Task.FromResult(response);
    }

    public Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        // For this MVP, we'll validate against the configured API key
        // In production, you would validate against a database of valid keys
        return Task.FromResult(apiKey == _apiKeySettings.ApiKey);
    }

    private string GenerateSecureApiKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        
        // Generate a 32-character API key
        var apiKey = new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        
        return $"fcg_{apiKey}";
    }
}
