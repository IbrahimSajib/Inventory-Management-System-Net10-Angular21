using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Authentication & Authorization
    IGenericRepository<User> Users { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<UserRole> UserRoles { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }

    // Master Data
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<UnitOfMeasure> UnitOfMeasures { get; }
    IGenericRepository<Customer> Customers { get; }
    IGenericRepository<Vendor> Vendors { get; }

    // Inventory
    IGenericRepository<Item> Items { get; }
    IGenericRepository<ItemStock> ItemStocks { get; }

    // Transactions
    IGenericRepository<PurchaseOrder> PurchaseOrders { get; }
    IGenericRepository<PurchaseOrderItem> PurchaseOrderItems { get; }
    IGenericRepository<SalesOrder> SalesOrders { get; }
    IGenericRepository<SalesOrderItem> SalesOrderItems { get; }
    IGenericRepository<Quotation> Quotations { get; }
    IGenericRepository<QuotationItem> QuotationItems { get; }

    // Save changes
    Task<int> SaveChangesAsync();
    int SaveChanges();
}