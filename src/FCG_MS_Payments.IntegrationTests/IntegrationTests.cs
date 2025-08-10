using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Moq;
using FCG_MS_Payments.Application.DTOs;
using FCG_MS_Payments.Application.Interfaces;
using FCG_MS_Payments.Application.Services;
using FCG_MS_Payments.Domain.Interfaces;
using FCG_MS_Payments.Domain.Entities;
using FCG_MS_Payments.Domain.Enums;
using FCG_MS_Payments.Infrastructure.Services;
using FCG_MS_Payments.Infrastructure.Repositories;
using FCG_MS_Payments.Domain.Exceptions;

namespace FCG_MS_Payments.IntegrationTests;

public class IntegrationTests
{
    private readonly ServiceProvider _serviceProvider;

    public IntegrationTests()
    {
        var services = new ServiceCollection();
        
        // Configure AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Payment, PaymentResponse>();
        });
        services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
        
        // Configure test services
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymentRepository, InMemoryPaymentRepository>();
        services.AddScoped<IPaymentApplicationService, PaymentApplicationService>();
        
        // Mock IStripeService for testing
        var mockStripeService = new Mock<IStripeService>();
        mockStripeService.Setup(x => x.CreatePaymentIntentAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(new StripePaymentIntent { Id = "pi_test_123", Status = "requires_payment_method", ClientSecret = "pi_test_123_secret" });
        mockStripeService.Setup(x => x.ConfirmPaymentIntentAsync(It.IsAny<string>()))
            .ReturnsAsync(new StripePaymentIntent { Id = "pi_test_123", Status = "succeeded", ClientSecret = "pi_test_123_secret" });
        services.AddSingleton<IStripeService>(mockStripeService.Object);
        
        services.AddLogging(builder => builder.AddConsole());
        
        _serviceProvider = services.BuildServiceProvider();
    }

    #region Video Game Payment Integration Tests

    [Fact]
    public async Task CompletePaymentFlow_WithCyberpunk2077_ShouldSucceed()
    {
        // Arrange - Cyberpunk 2077 Ultimate Edition
        var createRequest = new CreatePaymentRequest
        {
            Amount = 79.99m,
            Currency = "usd",
            CustomerId = "cus_cyberpunk_fan",
            Description = "Cyberpunk 2077 Ultimate Edition - Digital Download",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Cyberpunk 2077" },
                { "edition", "Ultimate" },
                { "platform", "PC" },
                { "genre", "RPG" },
                { "developer", "CD Projekt Red" }
            }
        };

        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act - Create Payment
        var createResult = await applicationService.CreatePaymentAsync(createRequest);

        // Assert - Create Payment
        createResult.Should().NotBeNull();
        createResult.Amount.Should().Be(79.99m);
        createResult.Currency.Should().Be("usd");
        createResult.CustomerId.Should().Be("cus_cyberpunk_fan");
        createResult.Description.Should().Contain("Cyberpunk 2077");
        createResult.Status.Should().Be(PaymentStatus.Pending);
        createResult.Metadata["game_title"].Should().Be("Cyberpunk 2077");

        // Act - Confirm Payment
        var confirmRequest = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = createResult.StripePaymentIntentId
        };

        var confirmResult = await applicationService.ConfirmPaymentAsync(confirmRequest);

        // Assert - Confirm Payment
        confirmResult.Should().NotBeNull();
        confirmResult.Status.Should().Be(PaymentStatus.Succeeded);

        // Act - Get Payment by ID
        var getResult = await applicationService.GetPaymentAsync(createResult.Id);

        // Assert - Get Payment
        getResult.Should().NotBeNull();
        getResult.Id.Should().Be(createResult.Id);
        getResult.Status.Should().Be(PaymentStatus.Succeeded);
    }

    [Fact]
    public async Task CompletePaymentFlow_WithStardewValley_ShouldSucceed()
    {
        // Arrange - Stardew Valley
        var createRequest = new CreatePaymentRequest
        {
            Amount = 14.99m,
            Currency = "usd",
            CustomerId = "cus_stardew_fan",
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

        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act - Create Payment
        var createResult = await applicationService.CreatePaymentAsync(createRequest);

        // Assert - Create Payment
        createResult.Should().NotBeNull();
        createResult.Amount.Should().Be(14.99m);
        createResult.Metadata["genre"].Should().Be("Simulation");

        // Act - Get Payment by Stripe ID
        var getByStripeResult = await applicationService.GetPaymentByStripeIdAsync(createResult.StripePaymentIntentId);

        // Assert - Get Payment by Stripe ID
        getByStripeResult.Should().NotBeNull();
        getByStripeResult.StripePaymentIntentId.Should().Be(createResult.StripePaymentIntentId);
    }

    [Fact]
    public async Task CompletePaymentFlow_WithFortniteBattlePass_ShouldSucceed()
    {
        // Arrange - Fortnite Battle Pass
        var createRequest = new CreatePaymentRequest
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

        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act - Create Payment
        var createResult = await applicationService.CreatePaymentAsync(createRequest);

        // Assert - Create Payment
        createResult.Should().NotBeNull();
        createResult.Amount.Should().Be(9.99m);
        createResult.Metadata["item_type"].Should().Be("Battle Pass");

        // Act - Get Payments by Customer
        var getByCustomerResult = await applicationService.GetPaymentsByCustomerAsync(createRequest.CustomerId);

        // Assert - Get Payments by Customer
        getByCustomerResult.Should().NotBeNull();
        getByCustomerResult.Should().Contain(p => p.CustomerId == createRequest.CustomerId);
    }

    [Fact]
    public async Task CompletePaymentFlow_WithWitcher3DLC_ShouldSucceed()
    {
        // Arrange - The Witcher 3 DLC
        var createRequest = new CreatePaymentRequest
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

        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act - Create Payment
        var createResult = await applicationService.CreatePaymentAsync(createRequest);

        // Assert - Create Payment
        createResult.Should().NotBeNull();
        createResult.Amount.Should().Be(19.99m);
        createResult.Metadata["item_type"].Should().Be("DLC");
        createResult.Metadata["dlc_name"].Should().Be("Blood and Wine");
    }

    [Fact]
    public async Task CompletePaymentFlow_WithGTASharkCard_ShouldSucceed()
    {
        // Arrange - GTA V Shark Cards
        var createRequest = new CreatePaymentRequest
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

        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act - Create Payment
        var createResult = await applicationService.CreatePaymentAsync(createRequest);

        // Assert - Create Payment
        createResult.Should().NotBeNull();
        createResult.Amount.Should().Be(24.99m);
        createResult.Metadata["item_type"].Should().Be("In-Game Currency");
        createResult.Metadata["currency_name"].Should().Be("GTA Dollars");
    }

    #endregion

    #region Error Handling Integration Tests

    [Fact]
    public async Task CreatePayment_WithInvalidAmount_ShouldThrowException()
    {
        // Arrange
        var createRequest = new CreatePaymentRequest
        {
            Amount = 0m, // Invalid amount
            Currency = "usd",
            CustomerId = "cus_test",
            Description = "Invalid payment test"
        };

        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act & Assert
        await Assert.ThrowsAsync<PaymentException>(() => 
            applicationService.CreatePaymentAsync(createRequest));
    }

    [Fact]
    public async Task GetPayment_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act & Assert
        await Assert.ThrowsAsync<PaymentException>(() => 
            applicationService.GetPaymentAsync(invalidId));
    }

    [Fact]
    public async Task ConfirmPayment_WithInvalidStripeId_ShouldThrowException()
    {
        // Arrange
        var confirmRequest = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = "pi_invalid_123"
        };

        var applicationService = _serviceProvider.GetRequiredService<IPaymentApplicationService>();

        // Act & Assert
        await Assert.ThrowsAsync<PaymentException>(() => 
            applicationService.ConfirmPaymentAsync(confirmRequest));
    }

    #endregion
}
