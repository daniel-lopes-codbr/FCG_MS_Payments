namespace FCG.MS.Payments.Application.DTOs;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "usd";
    public string ExternalProductId { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
