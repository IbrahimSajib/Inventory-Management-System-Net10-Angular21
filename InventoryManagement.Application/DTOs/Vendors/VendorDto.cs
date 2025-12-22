namespace InventoryManagement.Application.DTOs.Vendors;

public class VendorDto
{
    public int Id { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public DateTime CreatedAt { get; set; }
}