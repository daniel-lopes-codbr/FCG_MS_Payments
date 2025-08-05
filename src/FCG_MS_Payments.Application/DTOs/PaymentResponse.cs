using FCG_MS_Payments.Domain.Enums;

namespace FCG_MS_Payments.Application.DTOs;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string? ClientSecret { get; set; }
} 