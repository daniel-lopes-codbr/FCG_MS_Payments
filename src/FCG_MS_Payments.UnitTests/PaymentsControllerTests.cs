using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FCG_MS_Payments.Api.Controllers;
using FCG_MS_Payments.Api.Models;
using FCG_MS_Payments.Application.DTOs;
using FCG_MS_Payments.Application.Interfaces;
using FCG_MS_Payments.Domain.Exceptions;
using FCG_MS_Payments.Domain.Enums;

namespace FCG_MS_Payments.UnitTests;

public class PaymentsControllerTests
{
    private readonly Mock<IPaymentApplicationService> _mockPaymentService;
    private readonly Mock<ILogger<PaymentsController>> _mockLogger;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _mockPaymentService = new Mock<IPaymentApplicationService>();
        _mockLogger = new Mock<ILogger<PaymentsController>>();
        _controller = new PaymentsController(_mockPaymentService.Object, _mockLogger.Object);
    }

    #region Create Payment Tests

    [Fact]
    public async Task CreatePayment_WithValidVideoGamePurchase_ShouldReturnSuccess()
    {
        // Arrange - Cyberpunk 2077 Ultimate Edition
        var request = new CreatePaymentRequest
        {
            Amount = 79.99m,
            Currency = "usd",
            CustomerId = "cus_gamer123",
            Description = "Cyberpunk 2077 Ultimate Edition - Digital Download",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Cyberpunk 2077" },
                { "edition", "Ultimate" },
                { "platform", "PC" },
                { "genre", "RPG" }
            }
        };

        var expectedResponse = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_cyberpunk_123",
            Amount = 79.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_gamer123",
            Description = "Cyberpunk 2077 Ultimate Edition - Digital Download",
            CreatedAt = DateTime.UtcNow,
            ClientSecret = "pi_cyberpunk_123_secret_test",
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Payment created successfully");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Amount.Should().Be(79.99m);
        apiResponse.Data.Description.Should().Contain("Cyberpunk 2077");
        apiResponse.Data.Metadata["game_title"].Should().Be("Cyberpunk 2077");
    }

    [Fact]
    public async Task CreatePayment_WithValidIndieGamePurchase_ShouldReturnSuccess()
    {
        // Arrange - Stardew Valley
        var request = new CreatePaymentRequest
        {
            Amount = 14.99m,
            Currency = "usd",
            CustomerId = "cus_indie_fan",
            Description = "Stardew Valley - Farming Simulator",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Stardew Valley" },
                { "edition", "Standard" },
                { "platform", "Nintendo Switch" },
                { "genre", "Simulation" },
                { "developer", "ConcernedApe" }
            }
        };

        var expectedResponse = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_stardew_456",
            Amount = 14.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_indie_fan",
            Description = "Stardew Valley - Farming Simulator",
            CreatedAt = DateTime.UtcNow,
            ClientSecret = "pi_stardew_456_secret_test",
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data!.Amount.Should().Be(14.99m);
        apiResponse.Data.Metadata["genre"].Should().Be("Simulation");
    }

    [Fact]
    public async Task CreatePayment_WithValidBattlePassPurchase_ShouldReturnSuccess()
    {
        // Arrange - Fortnite Battle Pass
        var request = new CreatePaymentRequest
        {
            Amount = 9.99m,
            Currency = "usd",
            CustomerId = "cus_fortnite_player",
            Description = "Fortnite Chapter 5 Battle Pass",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Fortnite" },
                { "item_type", "Battle Pass" },
                { "season", "Chapter 5" },
                { "platform", "Cross-Platform" },
                { "genre", "Battle Royale" },
                { "duration", "90 days" }
            }
        };

        var expectedResponse = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_fortnite_bp_789",
            Amount = 9.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_fortnite_player",
            Description = "Fortnite Chapter 5 Battle Pass",
            CreatedAt = DateTime.UtcNow,
            ClientSecret = "pi_fortnite_bp_789_secret_test",
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data!.Amount.Should().Be(9.99m);
        apiResponse.Data.Metadata["item_type"].Should().Be("Battle Pass");
    }

    [Fact]
    public async Task CreatePayment_WithValidDLCPurchase_ShouldReturnSuccess()
    {
        // Arrange - The Witcher 3 DLC
        var request = new CreatePaymentRequest
        {
            Amount = 19.99m,
            Currency = "usd",
            CustomerId = "cus_witcher_fan",
            Description = "The Witcher 3: Blood and Wine DLC",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "The Witcher 3" },
                { "item_type", "DLC" },
                { "dlc_name", "Blood and Wine" },
                { "platform", "PC" },
                { "genre", "RPG" },
                { "content_size", "15 GB" }
            }
        };

        var expectedResponse = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_witcher_dlc_101",
            Amount = 19.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_witcher_fan",
            Description = "The Witcher 3: Blood and Wine DLC",
            CreatedAt = DateTime.UtcNow,
            ClientSecret = "pi_witcher_dlc_101_secret_test",
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data!.Amount.Should().Be(19.99m);
        apiResponse.Data.Metadata["item_type"].Should().Be("DLC");
    }

    [Fact]
    public async Task CreatePayment_WithValidInGameCurrencyPurchase_ShouldReturnSuccess()
    {
        // Arrange - GTA V Shark Cards
        var request = new CreatePaymentRequest
        {
            Amount = 24.99m,
            Currency = "usd",
            CustomerId = "cus_gta_player",
            Description = "GTA V Shark Card - $1,250,000",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Grand Theft Auto V" },
                { "item_type", "In-Game Currency" },
                { "currency_amount", "1250000" },
                { "currency_name", "GTA Dollars" },
                { "platform", "PlayStation 5" },
                { "genre", "Action-Adventure" }
            }
        };

        var expectedResponse = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_gta_shark_202",
            Amount = 24.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_gta_player",
            Description = "GTA V Shark Card - $1,250,000",
            CreatedAt = DateTime.UtcNow,
            ClientSecret = "pi_gta_shark_202_secret_test",
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data!.Amount.Should().Be(24.99m);
        apiResponse.Data.Metadata["item_type"].Should().Be("In-Game Currency");
    }

    [Fact]
    public async Task CreatePayment_WithPaymentException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 0m, // Invalid amount
            Currency = "usd",
            CustomerId = "cus_test",
            Description = "Invalid payment test"
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(request))
            .ThrowsAsync(new PaymentException("Invalid payment amount"));

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Invalid payment amount");
    }

    [Fact]
    public async Task CreatePayment_WithUnexpectedException_ShouldReturnInternalServerError()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 29.99m,
            Currency = "usd",
            CustomerId = "cus_test",
            Description = "Test payment"
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(request))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.CreatePayment(request);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        
        var apiResponse = statusCodeResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("An unexpected error occurred");
    }

    #endregion

    #region Confirm Payment Tests

    [Fact]
    public async Task ConfirmPayment_WithValidStripeId_ShouldReturnSuccess()
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = "pi_cyberpunk_123"
        };

        var expectedResponse = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_cyberpunk_123",
            Amount = 79.99m,
            Currency = "usd",
            Status = PaymentStatus.Succeeded,
            CustomerId = "cus_gamer123",
            Description = "Cyberpunk 2077 Ultimate Edition - Digital Download",
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Cyberpunk 2077" }
            }
        };

        _mockPaymentService.Setup(x => x.ConfirmPaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ConfirmPayment(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Payment confirmed successfully");
        apiResponse.Data!.Status.Should().Be(PaymentStatus.Succeeded);
    }

    [Fact]
    public async Task ConfirmPayment_WithPaymentException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = "pi_invalid_123"
        };

        _mockPaymentService.Setup(x => x.ConfirmPaymentAsync(request))
            .ThrowsAsync(new PaymentException("Payment intent not found"));

        // Act
        var result = await _controller.ConfirmPayment(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Payment intent not found");
    }

    #endregion

    #region Get Payment Tests

    [Fact]
    public async Task GetPayment_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedResponse = new PaymentResponse
        {
            Id = paymentId,
            StripePaymentIntentId = "pi_stardew_456",
            Amount = 14.99m,
            Currency = "usd",
            Status = PaymentStatus.Succeeded,
            CustomerId = "cus_indie_fan",
            Description = "Stardew Valley - Farming Simulator",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Stardew Valley" }
            }
        };

        _mockPaymentService.Setup(x => x.GetPaymentAsync(paymentId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetPayment(paymentId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data!.Id.Should().Be(paymentId);
        apiResponse.Data.Amount.Should().Be(14.99m);
    }

    [Fact]
    public async Task GetPayment_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        _mockPaymentService.Setup(x => x.GetPaymentAsync(paymentId))
            .ThrowsAsync(new PaymentException("Payment not found"));

        // Act
        var result = await _controller.GetPayment(paymentId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Payment not found");
    }

    [Fact]
    public async Task GetPaymentByStripeId_WithValidStripeId_ShouldReturnSuccess()
    {
        // Arrange
        var stripeId = "pi_fortnite_bp_789";
        var expectedResponse = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = stripeId,
            Amount = 9.99m,
            Currency = "usd",
            Status = PaymentStatus.Succeeded,
            CustomerId = "cus_fortnite_player",
            Description = "Fortnite Chapter 5 Battle Pass",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Fortnite" }
            }
        };

        _mockPaymentService.Setup(x => x.GetPaymentByStripeIdAsync(stripeId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetPaymentByStripeId(stripeId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PaymentResponse>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data!.StripePaymentIntentId.Should().Be(stripeId);
        apiResponse.Data.Amount.Should().Be(9.99m);
    }

    #endregion

    #region Get Payments By Customer Tests

    [Fact]
    public async Task GetPaymentsByCustomer_WithValidCustomerId_ShouldReturnSuccess()
    {
        // Arrange
        var customerId = "cus_gamer123";
        var expectedPayments = new List<PaymentResponse>
        {
            new PaymentResponse
            {
                Id = Guid.NewGuid(),
                StripePaymentIntentId = "pi_cyberpunk_123",
                Amount = 79.99m,
                Currency = "usd",
                Status = PaymentStatus.Succeeded,
                CustomerId = customerId,
                Description = "Cyberpunk 2077 Ultimate Edition",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Metadata = new Dictionary<string, string>
                {
                    { "product_type", "video_game" },
                    { "game_title", "Cyberpunk 2077" }
                }
            },
            new PaymentResponse
            {
                Id = Guid.NewGuid(),
                StripePaymentIntentId = "pi_witcher_dlc_101",
                Amount = 19.99m,
                Currency = "usd",
                Status = PaymentStatus.Succeeded,
                CustomerId = customerId,
                Description = "The Witcher 3: Blood and Wine DLC",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Metadata = new Dictionary<string, string>
                {
                    { "product_type", "video_game" },
                    { "game_title", "The Witcher 3" }
                }
            }
        };

        _mockPaymentService.Setup(x => x.GetPaymentsByCustomerAsync(customerId))
            .ReturnsAsync(expectedPayments);

        // Act
        var result = await _controller.GetPaymentsByCustomer(customerId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<PaymentResponse>>>().Subject;
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Should().HaveCount(2);
        apiResponse.Data!.First().CustomerId.Should().Be(customerId);
    }

    [Fact]
    public async Task GetPaymentsByCustomer_WithPaymentException_ShouldReturnBadRequest()
    {
        // Arrange
        var customerId = "cus_invalid";

        _mockPaymentService.Setup(x => x.GetPaymentsByCustomerAsync(customerId))
            .ThrowsAsync(new PaymentException("Customer not found"));

        // Act
        var result = await _controller.GetPaymentsByCustomer(customerId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<IEnumerable<PaymentResponse>>>().Subject;
        
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Customer not found");
    }

    #endregion
}
