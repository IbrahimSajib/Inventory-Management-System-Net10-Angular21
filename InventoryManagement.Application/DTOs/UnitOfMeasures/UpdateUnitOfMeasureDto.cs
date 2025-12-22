using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.UnitOfMeasures;

public class UpdateUnitOfMeasureDto
{
    [Required]
    [StringLength(100)]
    public string UnitName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? ShortName { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}