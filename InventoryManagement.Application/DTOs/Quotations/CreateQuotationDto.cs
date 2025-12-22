using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Quotations;

public class CreateQuotationDto
{
    [Required]
    public DateTime QuotationDate { get; set; }

    [Required]
    public DateTime ValidUntil { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(2000)]
    public string? TermsAndConditions { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required")]
    public List<CreateQuotationItemDto> Items { get; set; } = new List<CreateQuotationItemDto>();
}

public class CreateQuotationItemDto
{
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