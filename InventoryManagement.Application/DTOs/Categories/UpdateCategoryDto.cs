using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Categories;

public class UpdateCategoryDto
{
    [Required]
    [StringLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
}