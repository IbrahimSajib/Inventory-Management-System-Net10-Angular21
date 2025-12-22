using System.ComponentModel.DataAnnotations;
using InventoryManagement.Core.Enums;

namespace InventoryManagement.Application.DTOs.SalesOrders;

public class UpdateSalesOrderDto
{
    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }

    [StringLength(500)]
    public string? ShippingAddress { get; set; }

    [Required]
    public List<UpdateSalesOrderItemDto> Items { get; set; } = new List<UpdateSalesOrderItemDto>();
}

public class UpdateSalesOrderItemDto
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