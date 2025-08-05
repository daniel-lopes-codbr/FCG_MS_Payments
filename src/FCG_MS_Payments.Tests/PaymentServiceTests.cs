using Xunit;
using Moq;
using FluentAssertions;
using FCG_MS_Payments.Domain.Interfaces;
using FCG_MS_Payments.Domain.Entities;
using FCG_MS_Payments.Domain.Enums;
using FCG_MS_Payments.Infrastructure.Services;
using FCG_MS_Payments.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace FCG_MS_Payments.Tests;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _mockRepository;
    private readonly Mock<IStripeService> _mockStripeService;
    private readonly Mock<ILogger<PaymentService>> _mockLogger;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _mockRepository = new Mock<IPaymentRepository>();
        _mockStripeService = new Mock<IStripeService>();
        _mockLogger = new Mock<ILogger<PaymentService>>();
        
        _paymentService = new PaymentService(_mockRepository.Object, _mockStripeService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldCreatePaymentSuccessfully()
    {
        // Arrange
        var amount = 100.00m;
        var currency = "usd";
        var customerId = "cus_test123";
        var description = "Test payment";
        var metadata = new Dictionary<string, string> { { "test", "value" } };

        var paymentIntent = new StripePaymentIntent
        {
            Id = "pi_test123",
            Status = "requires_payment_method",
            ClientSecret = "pi_test123_secret_test"
        };

        var expectedPayment = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = paymentIntent.Id,
            Amount = amount,
            Currency = currency,
            CustomerId = customerId,
            Description = description,
            Status = PaymentStatus.Pending,
            Metadata = metadata
        };

        _mockStripeService.Setup(x => x.CreatePaymentIntentAsync(amount, currency, customerId, description, metadata))
            .ReturnsAsync(paymentIntent);

        _mockStripeService.Setup(x => x.MapStripeStatusToPaymentStatus(paymentIntent.Status))
            .Returns(PaymentStatus.Pending);

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Payment>()))
            .ReturnsAsync(expectedPayment);

        // Act
        var result = await _paymentService.CreatePaymentAsync(amount, currency, customerId, description, metadata);

        // Assert
        result.Should().NotBeNull();
        result.StripePaymentIntentId.Should().Be(paymentIntent.Id);
        result.Amount.Should().Be(amount);
        result.Currency.Should().Be(currency);
        result.CustomerId.Should().Be(customerId);
        result.Description.Should().Be(description);
        result.Status.Should().Be(PaymentStatus.Pending);

        _mockStripeService.Verify(x => x.CreatePaymentIntentAsync(amount, currency, customerId, description, metadata), Times.Once);
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Payment>()), Times.Once);
    }
} 