using InventoryManagement.Core.Common;

namespace InventoryManagement.Core.Entities;

public class Item : BaseEntity
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int UnitOfMeasureId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation Properties
    public Category Category { get; set; } = null!;
    public UnitOfMeasure UnitOfMeasure { get; set; } = null!;
    public ItemStock? ItemStock { get; set; }
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
    public ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
}