using FCG.MS.Payments.Application.DTOs;

namespace FCG.MS.Payments.Application.Interfaces;

public interface IPaymentService
{
    Task<CreateProductResponse> CreateProductAsync(CreateProductRequest request);
    Task<IEnumerable<ProductListResponse>> GetAllProductsAsync();
    Task<ProductListResponse?> GetProductByExternalIdAsync(string externalProductId);
    Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request);
    Task<CustomerListResponse?> GetCustomerAsync(string externalCustomerId);
    Task<CustomerListResponse?> GetCustomerByExternalIdAsync(string externalCustomerId);
    Task<CreateCheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequest request);
    Task<CreatePaymentResponse> CreatePaymentIntentAsync(CreatePaymentRequest request);
    Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentIntentId);
}
