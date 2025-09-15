namespace FCG.MS.Payments.Application.DTOs;

public class CreateCustomerResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ExternalCustomerId { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
