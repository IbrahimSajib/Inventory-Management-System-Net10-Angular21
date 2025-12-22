namespace InventoryManagement.Application.DTOs.Items;

public class ItemDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int UnitOfMeasureId { get; set; }
    public string UnitOfMeasureName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public string? ImageUrl { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}