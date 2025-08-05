using FCG_MS_Payments.Domain.Enums;

namespace FCG_MS_Payments.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    public PaymentStatus Status { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();

    public Payment()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Status = PaymentStatus.Pending;
    }

    public void UpdateStatus(PaymentStatus newStatus, string? errorMessage = null)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        ErrorMessage = errorMessage;
    }

    public bool IsCompleted => Status == PaymentStatus.Succeeded || Status == PaymentStatus.Failed || Status == PaymentStatus.Cancelled;
    public bool IsSuccessful => Status == PaymentStatus.Succeeded;
} 