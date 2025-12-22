using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Items;

public class CreateItemDto
{
    [Required]
    [StringLength(50)]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string ItemName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int UnitOfMeasureId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
    public decimal UnitPrice { get; set; }

    public decimal? ReorderLevel { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public decimal InitialQuantity { get; set; } = 0;
}