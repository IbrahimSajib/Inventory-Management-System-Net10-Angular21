using InventoryManagement.Core.Common;
using InventoryManagement.Core.Enums;

namespace InventoryManagement.Core.Entities;

public class Quotation : BaseEntity
{
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public int CustomerId { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public decimal TotalAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }

    // Navigation Properties
    public Customer Customer { get; set; } = null!;
    public ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
}