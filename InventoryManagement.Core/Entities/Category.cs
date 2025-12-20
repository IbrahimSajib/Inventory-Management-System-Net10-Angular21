using InventoryManagement.Core.Common;

namespace InventoryManagement.Core.Entities;

public class Category : BaseEntity
{
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<Item> Items { get; set; } = new List<Item>();
}