using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;
using FCG_MS_Payments.Application.DTOs;
using FCG_MS_Payments.Application.Validators;

namespace FCG_MS_Payments.UnitTests;

public class ValidatorTests
{
    private readonly CreatePaymentRequestValidator _createPaymentValidator;
    private readonly ConfirmPaymentRequestValidator _confirmPaymentValidator;

    public ValidatorTests()
    {
        _createPaymentValidator = new CreatePaymentRequestValidator();
        _confirmPaymentValidator = new ConfirmPaymentRequestValidator();
    }

    #region Create Payment Request Validator Tests

    [Fact]
    public void CreatePaymentRequest_WithValidVideoGameData_ShouldPassValidation()
    {
        // Arrange - Elden Ring
        var request = new CreatePaymentRequest
        {
            Amount = 59.99m,
            Currency = "usd",
            CustomerId = "cus_elden_ring_fan",
            Description = "Elden Ring - Digital Deluxe Edition",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Elden Ring" },
                { "edition", "Digital Deluxe" },
                { "platform", "PlayStation 5" },
                { "genre", "Action RPG" },
                { "developer", "FromSoftware" }
            }
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePaymentRequest_WithValidIndieGameData_ShouldPassValidation()
    {
        // Arrange - Hollow Knight
        var request = new CreatePaymentRequest
        {
            Amount = 14.99m,
            Currency = "usd",
            CustomerId = "cus_hollow_knight_fan",
            Description = "Hollow Knight - Metroidvania Adventure",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Hollow Knight" },
                { "edition", "Standard" },
                { "platform", "Nintendo Switch" },
                { "genre", "Metroidvania" },
                { "developer", "Team Cherry" }
            }
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePaymentRequest_WithValidBattleRoyaleData_ShouldPassValidation()
    {
        // Arrange - Apex Legends
        var request = new CreatePaymentRequest
        {
            Amount = 19.99m,
            Currency = "usd",
            CustomerId = "cus_apex_player",
            Description = "Apex Legends - Season Pass",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Apex Legends" },
                { "item_type", "Season Pass" },
                { "season", "Season 20" },
                { "platform", "Cross-Platform" },
                { "genre", "Battle Royale" },
                { "developer", "Respawn Entertainment" }
            }
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePaymentRequest_WithValidMMORPGData_ShouldPassValidation()
    {
        // Arrange - World of Warcraft
        var request = new CreatePaymentRequest
        {
            Amount = 39.99m,
            Currency = "usd",
            CustomerId = "cus_wow_player",
            Description = "World of Warcraft: Dragonflight Expansion",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "World of Warcraft" },
                { "item_type", "Expansion" },
                { "expansion_name", "Dragonflight" },
                { "platform", "PC" },
                { "genre", "MMORPG" },
                { "developer", "Blizzard Entertainment" }
            }
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePaymentRequest_WithValidRacingGameData_ShouldPassValidation()
    {
        // Arrange - Forza Horizon 5
        var request = new CreatePaymentRequest
        {
            Amount = 69.99m,
            Currency = "usd",
            CustomerId = "cus_forza_fan",
            Description = "Forza Horizon 5 - Premium Edition",
            Metadata = new Dictionary<string, string>
            {
                { "product_type", "video_game" },
                { "game_title", "Forza Horizon 5" },
                { "edition", "Premium" },
                { "platform", "Xbox Series X" },
                { "genre", "Racing" },
                { "developer", "Playground Games" },
                { "includes_dlc", "true" }
            }
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePaymentRequest_WithZeroAmount_ShouldFailValidation()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 0m,
            Currency = "usd",
            CustomerId = "cus_test",
            Description = "Test game purchase"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void CreatePaymentRequest_WithNegativeAmount_ShouldFailValidation()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = -10.00m,
            Currency = "usd",
            CustomerId = "cus_test",
            Description = "Test game purchase"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void CreatePaymentRequest_WithEmptyCustomerId_ShouldFailValidation()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 29.99m,
            Currency = "usd",
            CustomerId = "",
            Description = "Test game purchase"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void CreatePaymentRequest_WithEmptyDescription_ShouldFailValidation()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 29.99m,
            Currency = "usd",
            CustomerId = "cus_test",
            Description = ""
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void CreatePaymentRequest_WithInvalidCurrency_ShouldFailValidation()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 29.99m,
            Currency = "invalid",
            CustomerId = "cus_test",
            Description = "Test game purchase"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("eur")]
    [InlineData("gbp")]
    [InlineData("cad")]
    [InlineData("aud")]
    public void CreatePaymentRequest_WithValidCurrencies_ShouldPassValidation(string currency)
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 29.99m,
            Currency = currency,
            CustomerId = "cus_test",
            Description = "Test game purchase"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Currency);
    }

    #endregion

    #region Confirm Payment Request Validator Tests

    [Fact]
    public void ConfirmPaymentRequest_WithValidStripeId_ShouldPassValidation()
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = "pi_elden_ring_909"
        };

        // Act
        var result = _confirmPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ConfirmPaymentRequest_WithEmptyStripeId_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = ""
        };

        // Act
        var result = _confirmPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StripePaymentIntentId);
    }

    [Fact]
    public void ConfirmPaymentRequest_WithNullStripeId_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = null!
        };

        // Act
        var result = _confirmPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StripePaymentIntentId);
    }

    [Fact]
    public void ConfirmPaymentRequest_WithInvalidStripeIdFormat_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = "invalid_stripe_id"
        };

        // Act
        var result = _confirmPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StripePaymentIntentId);
    }

    [Theory]
    [InlineData("pi_1234567890abcdef")]
    [InlineData("pi_test_1234567890")]
    [InlineData("pi_elden_ring_909_secret_test")]
    public void ConfirmPaymentRequest_WithValidStripeIdFormats_ShouldPassValidation(string stripeId)
    {
        // Arrange
        var request = new ConfirmPaymentRequest
        {
            StripePaymentIntentId = stripeId
        };

        // Act
        var result = _confirmPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StripePaymentIntentId);
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public void CreatePaymentRequest_WithMaximumAmount_ShouldPassValidation()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 999999.99m,
            Currency = "usd",
            CustomerId = "cus_test",
            Description = "Expensive game bundle"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void CreatePaymentRequest_WithMinimumValidAmount_ShouldPassValidation()
    {
        // Arrange
        var request = new CreatePaymentRequest
        {
            Amount = 0.01m,
            Currency = "usd",
            CustomerId = "cus_test",
            Description = "Cheap DLC"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void CreatePaymentRequest_WithLongDescription_ShouldPassValidation()
    {
        // Arrange
        var longDescription = new string('A', 500); // 500 character description
        var request = new CreatePaymentRequest
        {
            Amount = 29.99m,
            Currency = "usd",
            CustomerId = "cus_test",
            Description = longDescription
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void CreatePaymentRequest_WithLongCustomerId_ShouldPassValidation()
    {
        // Arrange
        var longCustomerId = new string('C', 100); // 100 character customer ID
        var request = new CreatePaymentRequest
        {
            Amount = 29.99m,
            Currency = "usd",
            CustomerId = longCustomerId,
            Description = "Test game purchase"
        };

        // Act
        var result = _createPaymentValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CustomerId);
    }

    #endregion
}
