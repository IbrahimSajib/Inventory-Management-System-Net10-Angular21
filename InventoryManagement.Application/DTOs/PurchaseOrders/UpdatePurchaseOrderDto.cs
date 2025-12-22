using System.ComponentModel.DataAnnotations;
using InventoryManagement.Core.Enums;

namespace InventoryManagement.Application.DTOs.PurchaseOrders;

public class UpdatePurchaseOrderDto
{
    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    public int VendorId { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }

    [Required]
    public List<UpdatePurchaseOrderItemDto> Items { get; set; } = new List<UpdatePurchaseOrderItemDto>();
}

public class UpdatePurchaseOrderItemDto
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
}