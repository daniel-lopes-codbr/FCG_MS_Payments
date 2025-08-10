using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FCG_MS_Payments.Application.Services;
using FCG_MS_Payments.Application.DTOs;
using FCG_MS_Payments.Application.Interfaces;
using FCG_MS_Payments.Domain.Interfaces;
using FCG_MS_Payments.Domain.Entities;
using FCG_MS_Payments.Domain.Enums;
using FCG_MS_Payments.Domain.Exceptions;

namespace FCG_MS_Payments.UnitTests;

public class PaymentApplicationServiceTests
{
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<ILogger<PaymentApplicationService>> _mockLogger;
    private readonly PaymentApplicationService _applicationService;

    public PaymentApplicationServiceTests()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockLogger = new Mock<ILogger<PaymentApplicationService>>();
        
        // Create AutoMapper configuration for testing
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Payment, PaymentResponse>();
        });
        var mapper = mapperConfig.CreateMapper();
        
        _applicationService = new PaymentApplicationService(
            _mockPaymentService.Object, 
            mapper, 
            _mockLogger.Object);
    }

    #region Create Payment Tests

    [Fact]
    public async Task CreatePaymentAsync_WithValidVideoGamePurchase_ShouldReturnSuccess()
    {
        // Arrange - Red Dead Redemption 2
        var request = new CreatePaymentRequest
        {
            Amount = 59.99m,
            Currency = "usd",
            CustomerId = "cus_rdr2_fan",
            Description = "Red Dead Redemption 2 - Digital Edition",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Red Dead Redemption 2" },
                { "edition", "Digital" },
                { "platform", "Xbox Series X" },
                { "genre", "Action-Adventure" },
                { "developer", "Rockstar Games" }
            }
        };

        var paymentEntity = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_rdr2_303",
            Amount = 59.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_rdr2_fan",
            Description = "Red Dead Redemption 2 - Digital Edition",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(
            request.Amount, 
            request.Currency, 
            request.CustomerId, 
            request.Description, 
            request.Metadata))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.CreatePaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(59.99m);
        result.Currency.Should().Be("usd");
        result.CustomerId.Should().Be("cus_rdr2_fan");
        result.Description.Should().Contain("Red Dead Redemption 2");
        result.Status.Should().Be(PaymentStatus.Pending);
        result.Metadata["game_title"].Should().Be("Red Dead Redemption 2");
        result.Metadata["platform"].Should().Be("Xbox Series X");
    }

    [Fact]
    public async Task CreatePaymentAsync_WithValidIndieGamePurchase_ShouldReturnSuccess()
    {
        // Arrange - Celeste
        var request = new CreatePaymentRequest
        {
            Amount = 19.99m,
            Currency = "usd",
            CustomerId = "cus_celeste_fan",
            Description = "Celeste - Platformer Adventure",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Celeste" },
                { "edition", "Standard" },
                { "platform", "Nintendo Switch" },
                { "genre", "Platformer" },
                { "developer", "Matt Makes Games" }
            }
        };

        var paymentEntity = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_celeste_404",
            Amount = 19.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_celeste_fan",
            Description = "Celeste - Platformer Adventure",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(
            request.Amount, 
            request.Currency, 
            request.CustomerId, 
            request.Description, 
            request.Metadata))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.CreatePaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(19.99m);
        result.Currency.Should().Be("usd");
        result.CustomerId.Should().Be("cus_celeste_fan");
        result.Description.Should().Contain("Celeste");
        result.Status.Should().Be(PaymentStatus.Pending);
        result.Metadata["genre"].Should().Be("Platformer");
    }

    [Fact]
    public async Task CreatePaymentAsync_WithValidCosmeticPurchase_ShouldReturnSuccess()
    {
        // Arrange - League of Legends Skin
        var request = new CreatePaymentRequest
        {
            Amount = 9.99m,
            Currency = "usd",
            CustomerId = "cus_lol_player",
            Description = "League of Legends - Star Guardian Ahri Skin",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "League of Legends" },
                { "item_type", "Skin" },
                { "champion", "Ahri" },
                { "skin_line", "Star Guardian" },
                { "rarity", "Epic" },
                { "platform", "PC" },
                { "genre", "MOBA" }
            }
        };

        var paymentEntity = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_lol_skin_505",
            Amount = 9.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_lol_player",
            Description = "League of Legends - Star Guardian Ahri Skin",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(
            request.Amount, 
            request.Currency, 
            request.CustomerId, 
            request.Description, 
            request.Metadata))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.CreatePaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(9.99m);
        result.Currency.Should().Be("usd");
        result.CustomerId.Should().Be("cus_lol_player");
        result.Description.Should().Contain("Star Guardian Ahri");
        result.Status.Should().Be(PaymentStatus.Pending);
        result.Metadata["item_type"].Should().Be("Skin");
        result.Metadata["champion"].Should().Be("Ahri");
    }

    [Fact]
    public async Task CreatePaymentAsync_WithValidGameBundlePurchase_ShouldReturnSuccess()
    {
        // Arrange - Steam Bundle
        var request = new CreatePaymentRequest
        {
            Amount = 49.99m,
            Currency = "usd",
            CustomerId = "cus_steam_user",
            Description = "Steam Bundle - Indie Game Collection",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "bundle_name", "Indie Game Collection" },
                { "games_count", "5" },
                { "platform", "PC" },
                { "store", "Steam" },
                { "includes", "Hollow Knight, Celeste, Stardew Valley, Dead Cells, Hades" }
            }
        };

        var paymentEntity = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_steam_bundle_606",
            Amount = 49.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_steam_user",
            Description = "Steam Bundle - Indie Game Collection",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(
            request.Amount, 
            request.Currency, 
            request.CustomerId, 
            request.Description, 
            request.Metadata))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.CreatePaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(49.99m);
        result.Currency.Should().Be("usd");
        result.CustomerId.Should().Be("cus_steam_user");
        result.Description.Should().Contain("Indie Game Collection");
        result.Status.Should().Be(PaymentStatus.Pending);
        result.Metadata["bundle_name"].Should().Be("Indie Game Collection");
        result.Metadata["games_count"].Should().Be("5");
    }

    [Fact]
    public async Task CreatePaymentAsync_WithValidSubscriptionPurchase_ShouldReturnSuccess()
    {
        // Arrange - Xbox Game Pass
        var request = new CreatePaymentRequest
        {
            Amount = 14.99m,
            Currency = "usd",
            CustomerId = "cus_xbox_user",
            Description = "Xbox Game Pass Ultimate - Monthly Subscription",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "subscription_type", "Game Pass Ultimate" },
                { "duration", "1 month" },
                { "platform", "Xbox" },
                { "includes_gold", "true" },
                { "includes_ea_play", "true" },
                { "genre", "Subscription Service" }
            }
        };

        var paymentEntity = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_xbox_gp_707",
            Amount = 14.99m,
            Currency = "usd",
            Status = PaymentStatus.Pending,
            CustomerId = "cus_xbox_user",
            Description = "Xbox Game Pass Ultimate - Monthly Subscription",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };

        _mockPaymentService.Setup(x => x.CreatePaymentAsync(
            request.Amount, 
            request.Currency, 
            request.CustomerId, 
            request.Description, 
            request.Metadata))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.CreatePaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(14.99m);
        result.Currency.Should().Be("usd");
        result.CustomerId.Should().Be("cus_xbox_user");
        result.Description.Should().Contain("Game Pass Ultimate");
        result.Status.Should().Be(PaymentStatus.Pending);
        result.Metadata["subscription_type"].Should().Be("Game Pass Ultimate");
        result.Metadata["duration"].Should().Be("1 month");
    }

    #endregion

    #region Confirm Payment Tests

    [Fact]
    public async Task ConfirmPaymentAsync_WithValidStripeId_ShouldReturnSuccess()
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = "pi_rdr2_303"
        };

        var paymentEntity = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = "pi_rdr2_303",
            Amount = 59.99m,
            Currency = "usd",
            Status = PaymentStatus.Succeeded,
            CustomerId = "cus_rdr2_fan",
            Description = "Red Dead Redemption 2 - Digital Edition",
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Red Dead Redemption 2" }
            }
        };

        _mockPaymentService.Setup(x => x.ConfirmPaymentAsync(request.StripePaymentIntentId))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.ConfirmPaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Succeeded);
        result.StripePaymentIntentId.Should().Be("pi_rdr2_303");
        result.Amount.Should().Be(59.99m);
    }

    #endregion

    #region Get Payment Tests

    [Fact]
    public async Task GetPaymentAsync_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var paymentEntity = new Payment
        {
            Id = paymentId,
            StripePaymentIntentId = "pi_celeste_404",
            Amount = 19.99m,
            Currency = "usd",
            Status = PaymentStatus.Succeeded,
            CustomerId = "cus_celeste_fan",
            Description = "Celeste - Platformer Adventure",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Celeste" }
            }
        };

        _mockPaymentService.Setup(x => x.GetPaymentAsync(paymentId))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.GetPaymentAsync(paymentId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(paymentId);
        result.Amount.Should().Be(19.99m);
        result.Status.Should().Be(PaymentStatus.Succeeded);
        result.Description.Should().Contain("Celeste");
    }

    [Fact]
    public async Task GetPaymentAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        _mockPaymentService.Setup(x => x.GetPaymentAsync(paymentId))
            .ThrowsAsync(new PaymentException("Payment not found"));

        // Act & Assert
        await Assert.ThrowsAsync<PaymentException>(() => 
            _applicationService.GetPaymentAsync(paymentId));
    }

    [Fact]
    public async Task GetPaymentByStripeIdAsync_WithValidStripeId_ShouldReturnSuccess()
    {
        // Arrange
        var stripeId = "pi_lol_skin_505";
        var paymentEntity = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = stripeId,
            Amount = 9.99m,
            Currency = "usd",
            Status = PaymentStatus.Succeeded,
            CustomerId = "cus_lol_player",
            Description = "League of Legends - Star Guardian Ahri Skin",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "League of Legends" }
            }
        };

        _mockPaymentService.Setup(x => x.GetPaymentByStripeIdAsync(stripeId))
            .ReturnsAsync(paymentEntity);

        // Act
        var result = await _applicationService.GetPaymentByStripeIdAsync(stripeId);

        // Assert
        result.Should().NotBeNull();
        result.StripePaymentIntentId.Should().Be(stripeId);
        result.Amount.Should().Be(9.99m);
        result.Status.Should().Be(PaymentStatus.Succeeded);
        result.Description.Should().Contain("Star Guardian Ahri");
    }

    #endregion

    #region Get Payments By Customer Tests

    [Fact]
    public async Task GetPaymentsByCustomerAsync_WithValidCustomerId_ShouldReturnSuccess()
    {
        // Arrange
        var customerId = "cus_gamer123";
        var payments = new List<Payment>
        {
            new Payment
            {
                Id = Guid.NewGuid(),
                StripePaymentIntentId = "pi_rdr2_303",
                Amount = 59.99m,
                Currency = "usd",
                Status = PaymentStatus.Succeeded,
                CustomerId = customerId,
                Description = "Red Dead Redemption 2 - Digital Edition",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Metadata = new Dictionary<string, string>
                {
                    { "product_type", "video_game" },
                    { "game_title", "Red Dead Redemption 2" }
                }
            },
            new Payment
            {
                Id = Guid.NewGuid(),
                StripePaymentIntentId = "pi_celeste_404",
                Amount = 19.99m,
                Currency = "usd",
                Status = PaymentStatus.Succeeded,
                CustomerId = customerId,
                Description = "Celeste - Platformer Adventure",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Metadata = new Dictionary<string, string>
                {
                    { "product_type", "video_game" },
                    { "game_title", "Celeste" }
                }
            }
        };

        _mockPaymentService.Setup(x => x.GetPaymentsByCustomerAsync(customerId))
            .ReturnsAsync(payments);

        // Act
        var result = await _applicationService.GetPaymentsByCustomerAsync(customerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().CustomerId.Should().Be(customerId);
        result.First().Amount.Should().Be(59.99m);
        result.Last().Amount.Should().Be(19.99m);
    }

    #endregion
}
