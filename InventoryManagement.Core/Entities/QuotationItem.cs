using InventoryManagement.Core.Common;

namespace InventoryManagement.Core.Entities;

public class QuotationItem : BaseEntity
{
    public int QuotationId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string? Description { get; set; }

    // Navigation Properties
    public Quotation Quotation { get; set; } = null!;
    public Item Item { get; set; } = null!;
}