using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Vendors;

public class CreateVendorDto
{
    [Required]
    [StringLength(200)]
    public string VendorName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }
}