using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Data;
using InventoryManagement.Infrastructure.Repositories;

namespace InventoryManagement.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    // Authentication & Authorization
    private IGenericRepository<User>? _users;
    private IGenericRepository<Role>? _roles;
    private IGenericRepository<UserRole>? _userRoles;
    private IGenericRepository<RefreshToken>? _refreshTokens;

    // Master Data
    private IGenericRepository<Category>? _categories;
    private IGenericRepository<UnitOfMeasure>? _unitOfMeasures;
    private IGenericRepository<Customer>? _customers;
    private IGenericRepository<Vendor>? _vendors;

    // Inventory
    private IGenericRepository<Item>? _items;
    private IGenericRepository<ItemStock>? _itemStocks;

    // Transactions
    private IGenericRepository<PurchaseOrder>? _purchaseOrders;
    private IGenericRepository<PurchaseOrderItem>? _purchaseOrderItems;
    private IGenericRepository<SalesOrder>? _salesOrders;
    private IGenericRepository<SalesOrderItem>? _salesOrderItems;
    private IGenericRepository<Quotation>? _quotations;
    private IGenericRepository<QuotationItem>? _quotationItems;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Authentication & Authorization
    public IGenericRepository<User> Users =>
        _users ??= new GenericRepository<User>(_context);

    public IGenericRepository<Role> Roles =>
        _roles ??= new GenericRepository<Role>(_context);

    public IGenericRepository<UserRole> UserRoles =>
        _userRoles ??= new GenericRepository<UserRole>(_context);

    public IGenericRepository<RefreshToken> RefreshTokens =>
        _refreshTokens ??= new GenericRepository<RefreshToken>(_context);

    // Master Data
    public IGenericRepository<Category> Categories =>
        _categories ??= new GenericRepository<Category>(_context);

    public IGenericRepository<UnitOfMeasure> UnitOfMeasures =>
        _unitOfMeasures ??= new GenericRepository<UnitOfMeasure>(_context);

    public IGenericRepository<Customer> Customers =>
        _customers ??= new GenericRepository<Customer>(_context);

    public IGenericRepository<Vendor> Vendors =>
        _vendors ??= new GenericRepository<Vendor>(_context);

    // Inventory
    public IGenericRepository<Item> Items =>
        _items ??= new GenericRepository<Item>(_context);

    public IGenericRepository<ItemStock> ItemStocks =>
        _itemStocks ??= new GenericRepository<ItemStock>(_context);

    // Transactions
    public IGenericRepository<PurchaseOrder> PurchaseOrders =>
        _purchaseOrders ??= new GenericRepository<PurchaseOrder>(_context);

    public IGenericRepository<PurchaseOrderItem> PurchaseOrderItems =>
        _purchaseOrderItems ??= new GenericRepository<PurchaseOrderItem>(_context);

    public IGenericRepository<SalesOrder> SalesOrders =>
        _salesOrders ??= new GenericRepository<SalesOrder>(_context);

    public IGenericRepository<SalesOrderItem> SalesOrderItems =>
        _salesOrderItems ??= new GenericRepository<SalesOrderItem>(_context);

    public IGenericRepository<Quotation> Quotations =>
        _quotations ??= new GenericRepository<Quotation>(_context);

    public IGenericRepository<QuotationItem> QuotationItems =>
        _quotationItems ??= new GenericRepository<QuotationItem>(_context);

    // Save changes
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    // Dispose
    public void Dispose()
    {
        _context.Dispose();
    }
}