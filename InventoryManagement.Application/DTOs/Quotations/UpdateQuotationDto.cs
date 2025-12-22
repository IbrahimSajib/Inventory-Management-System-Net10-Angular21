using System.ComponentModel.DataAnnotations;
using InventoryManagement.Core.Enums;

namespace InventoryManagement.Application.DTOs.Quotations;

public class UpdateQuotationDto
{
    [Required]
    public DateTime QuotationDate { get; set; }

    [Required]
    public DateTime ValidUntil { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public QuotationStatus Status { get; set; }

    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(2000)]
    public string? TermsAndConditions { get; set; }

    [Required]
    public List<UpdateQuotationItemDto> Items { get; set; } = new List<UpdateQuotationItemDto>();
}

public class UpdateQuotationItemDto
{
    public int? Id { get; set; }

    [Required]
    public int ItemId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercent { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}