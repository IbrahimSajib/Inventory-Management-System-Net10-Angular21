using InventoryManagement.Core.Common;

namespace InventoryManagement.Core.Entities;

public class Customer : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }

    // Navigation Properties
    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
}