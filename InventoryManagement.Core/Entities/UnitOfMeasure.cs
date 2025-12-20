using InventoryManagement.Core.Common;

namespace InventoryManagement.Core.Entities;

public class UnitOfMeasure : BaseEntity
{
    public string UnitName { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<Item> Items { get; set; } = new List<Item>();
}