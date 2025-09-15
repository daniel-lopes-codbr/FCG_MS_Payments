namespace FCG.MS.Payments.Application.DTOs;

public class ProductListResponse
{
    public string Id { get; set; } = string.Empty;
    public string PriceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ExternalProductId { get; set; } = string.Empty;
}
