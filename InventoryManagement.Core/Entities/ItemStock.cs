using InventoryManagement.Core.Common;

namespace InventoryManagement.Core.Entities;

public class ItemStock : BaseEntity
{
    public int ItemId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable => QuantityOnHand - QuantityReserved;
    public DateTime LastRestockDate { get; set; }

    // Navigation Property
    public Item Item { get; set; } = null!;
}