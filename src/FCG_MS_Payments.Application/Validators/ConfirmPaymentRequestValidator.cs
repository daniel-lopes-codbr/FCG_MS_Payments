using FluentValidation;
using FCG_MS_Payments.Application.DTOs;

namespace FCG_MS_Payments.Application.Validators;

public class ConfirmPaymentRequestValidator : AbstractValidator<ConfirmPaymentRequest>
{
    public ConfirmPaymentRequestValidator()
    {
        RuleFor(x => x.StripePaymentIntentId)
            .NotEmpty()
            .WithMessage("Stripe Payment Intent ID is required");
    }
} 