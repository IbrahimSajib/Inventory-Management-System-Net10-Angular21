using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.PurchaseOrders;

public class CreatePurchaseOrderDto
{
    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    public int VendorId { get; set; }

    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required")]
    public List<CreatePurchaseOrderItemDto> Items { get; set; } = new List<CreatePurchaseOrderItemDto>();
}

public class CreatePurchaseOrderItemDto
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
}