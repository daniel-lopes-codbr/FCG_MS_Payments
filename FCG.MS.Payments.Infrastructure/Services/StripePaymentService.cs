using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using FCG.MS.Payments.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace FCG.MS.Payments.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;

    public StripePaymentService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<CreateProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var productOptions = new ProductCreateOptions
            {
                Name = request.Name,
                Description = request.Description,
                Metadata = new Dictionary<string, string>
                {
                    { "externalProductId", request.ExternalProductId },
                    { "imageUrl", request.ImageUrl }
                }
            };

            var productService = new ProductService();
            var product = await productService.CreateAsync(productOptions);

            var priceOptions = new PriceCreateOptions
            {
                Product = product.Id,
                UnitAmount = (long)(request.Price * 100), // Convert to cents
                Currency = request.Currency
            };

            var priceService = new PriceService();
            var price = await priceService.CreateAsync(priceOptions);

            return new CreateProductResponse
            {
                Id = product.Id,
                PriceId = price.Id,
                Name = product.Name,
                Description = product.Description ?? string.Empty,
                Price = request.Price,
                Currency = request.Currency,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = request.ImageUrl
            };
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error creating product: {ex.Message}");
        }
    }

    public async Task<IEnumerable<ProductListResponse>> GetAllProductsAsync()
    {
        try
        {
            var productService = new ProductService();
            var products = await productService.ListAsync(new ProductListOptions
            {
                Active = true,
                Limit = 100 // Limit to 100 products for performance
            });

            var productList = new List<ProductListResponse>();

            foreach (var product in products.Data)
            {
                // Get the price for each product
                var priceService = new PriceService();
                var prices = await priceService.ListAsync(new PriceListOptions
                {
                    Product = product.Id,
                    Active = true,
                    Limit = 1
                });

                var price = prices.Data.FirstOrDefault();
                if (price != null)
                {
                    // Extract metadata
                    var externalProductId = product.Metadata?.GetValueOrDefault("externalProductId") ?? string.Empty;
                    var imageUrl = product.Metadata?.GetValueOrDefault("imageUrl") ?? string.Empty;

                    productList.Add(new ProductListResponse
                    {
                        Id = product.Id,
                        PriceId = price.Id,
                        Name = product.Name,
                        Description = product.Description ?? string.Empty,
                        Price = (decimal)(price.UnitAmount ?? 0) / 100, // Convert from cents
                        Currency = price.Currency,
                        CreatedAt = product.Created,
                        ImageUrl = imageUrl,
                        ExternalProductId = externalProductId
                    });
                }
            }

            return productList;
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error retrieving products: {ex.Message}");
        }
    }

    public async Task<ProductListResponse?> GetProductByExternalIdAsync(string externalProductId)
    {
        try
        {
            var productService = new ProductService();
            var products = await productService.ListAsync(new ProductListOptions
            {
                Active = true,
                Limit = 100
            });

            // Find product by externalProductId in metadata
            var product = products.Data.FirstOrDefault(p => 
                p.Metadata?.GetValueOrDefault("externalProductId") == externalProductId);

            if (product == null)
            {
                return null;
            }

            // Get the price for the product
            var priceService = new PriceService();
            var prices = await priceService.ListAsync(new PriceListOptions
            {
                Product = product.Id,
                Active = true,
                Limit = 1
            });

            var price = prices.Data.FirstOrDefault();
            if (price == null)
            {
                return null;
            }

            // Extract metadata
            var imageUrl = product.Metadata?.GetValueOrDefault("imageUrl") ?? string.Empty;

            return new ProductListResponse
            {
                Id = product.Id,
                PriceId = price.Id,
                Name = product.Name,
                Description = product.Description ?? string.Empty,
                Price = (decimal)(price.UnitAmount ?? 0) / 100, // Convert from cents
                Currency = price.Currency,
                CreatedAt = product.Created,
                ImageUrl = imageUrl,
                ExternalProductId = externalProductId
            };
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error retrieving product: {ex.Message}");
        }
    }

    public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request)
    {
        try
        {
            var customerOptions = new CustomerCreateOptions
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Description = request.Description,
                Metadata = new Dictionary<string, string>
                {
                    { "externalCustomerId", request.ExternalCustomerId }
                }
            };

            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(customerOptions);

            return new CreateCustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name ?? string.Empty,
                Email = customer.Email ?? string.Empty,
                ExternalCustomerId = request.ExternalCustomerId,
                Phone = customer.Phone,
                Description = customer.Description,
                CreatedAt = customer.Created
            };
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error creating customer: {ex.Message}");
        }
    }

    public async Task<CustomerListResponse?> GetCustomerAsync(string externalCustomerId)
    {
        try
        {
            var customerService = new CustomerService();
            var customers = await customerService.ListAsync(new CustomerListOptions
            {
                Limit = 100
            });

            // Find customer by externalCustomerId in metadata
            var customer = customers.Data.FirstOrDefault(c => 
                c.Metadata?.GetValueOrDefault("externalCustomerId") == externalCustomerId);

            if (customer == null)
            {
                return null;
            }

            return new CustomerListResponse
            {
                Id = customer.Id,
                Name = customer.Name ?? string.Empty,
                Email = customer.Email ?? string.Empty,
                ExternalCustomerId = externalCustomerId,
                Phone = customer.Phone,
                Description = customer.Description,
                CreatedAt = customer.Created
            };
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error retrieving customer: {ex.Message}");
        }
    }

    public async Task<CustomerListResponse?> GetCustomerByExternalIdAsync(string externalCustomerId)
    {
        // This method is the same as GetCustomerAsync, but we keep both for clarity
        return await GetCustomerAsync(externalCustomerId);
    }

    public async Task<CreateCheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequest request)
    {
        try
        {
            // 1. Find customer by external ID
            var customer = await GetCustomerByExternalIdAsync(request.CustomerId);
            if (customer == null)
            {
                throw new Exception($"Customer with external ID '{request.CustomerId}' not found");
            }

            // 2. Find product by external ID
            var product = await GetProductByExternalIdAsync(request.ProductId);
            if (product == null)
            {
                throw new Exception($"Product with external ID '{request.ProductId}' not found");
            }

            // 3. Calculate total amount
            var totalAmount = product.Price * request.Quantity;

            // 4. Create checkout session in Stripe
            var checkoutSessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = product.PriceId, // Use the Stripe price ID
                        Quantity = request.Quantity,
                    },
                },
                Mode = "payment",
                       SuccessUrl = "http://localhost:4200/payment-success?success=true",
                CancelUrl = "http://localhost:4200/cart?canceled=true",
                Customer = customer.Id,
                Metadata = new Dictionary<string, string>
                {
                    { "externalCustomerId", request.CustomerId },
                    { "externalProductId", request.ProductId },
                    { "quantity", request.Quantity.ToString() }
                }
            };

            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(checkoutSessionOptions);

            return new CreateCheckoutResponse
            {
                SessionId = session.Id,
                Url = session.Url,
                Amount = totalAmount,
                Currency = product.Currency,
                Status = session.Status,
                CustomerId = request.CustomerId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error creating checkout: {ex.Message}");
        }
    }

    public async Task<CreatePaymentResponse> CreatePaymentIntentAsync(CreatePaymentRequest request)
    {
        try
        {
            // First, get the product to get its price
            var productService = new ProductService();
            var product = await productService.GetAsync(request.ProductId);

            var priceService = new PriceService();
            var prices = await priceService.ListAsync(new PriceListOptions
            {
                Product = product.Id,
                Active = true
            });

            var price = prices.Data.FirstOrDefault();
            if (price == null)
            {
                throw new Exception($"No active price found for product {request.ProductId}");
            }

            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = price.UnitAmount,
                Currency = price.Currency,
                Metadata = new Dictionary<string, string>
                {
                    { "product_id", request.ProductId }
                }
            };

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions);

            return new CreatePaymentResponse
            {
                Id = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Amount = (decimal)paymentIntent.Amount / 100, // Convert from cents
                Currency = paymentIntent.Currency,
                Status = paymentIntent.Status
            };
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error creating payment intent: {ex.Message}");
        }
    }

    public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentIntentId)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

            return new PaymentStatusResponse
            {
                Id = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = (decimal)paymentIntent.Amount / 100, // Convert from cents
                Currency = paymentIntent.Currency,
                CreatedAt = DateTime.UtcNow, // Using current time as fallback
                UpdatedAt = null // Stripe PaymentIntent doesn't have an Updated property in this version
            };
        }
        catch (StripeException ex)
        {
            throw new Exception($"Error retrieving payment status: {ex.Message}");
        }
    }
}
