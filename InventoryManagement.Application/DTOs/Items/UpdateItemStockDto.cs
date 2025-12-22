using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Items;

public class UpdateItemStockDto
{
    [Required]
    public decimal QuantityOnHand { get; set; }

    [Required]
    public decimal QuantityReserved { get; set; }
}