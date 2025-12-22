namespace InventoryManagement.Application.DTOs.UnitOfMeasures;

public class UnitOfMeasureDto
{
    public int Id { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}