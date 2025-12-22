using InventoryManagement.Core.Enums;

namespace InventoryManagement.Application.DTOs.Quotations;

public class QuotationDto
{
    public int Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public DateTime QuotationDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public QuotationStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuotationItemDto> Items { get; set; } = new List<QuotationItemDto>();
}

public class QuotationItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string? Description { get; set; }
}