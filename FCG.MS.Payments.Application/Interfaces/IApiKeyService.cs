using FCG.MS.Payments.Application.DTOs;

namespace FCG.MS.Payments.Application.Interfaces;

public interface IApiKeyService
{
    Task<GenerateApiKeyResponse> GenerateApiKeyAsync(GenerateApiKeyRequest request);
    Task<bool> ValidateApiKeyAsync(string apiKey);
}

