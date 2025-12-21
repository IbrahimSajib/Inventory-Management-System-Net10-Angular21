using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Authentication & Authorization
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }

    // Master Data
    public DbSet<Category> Category { get; set; }
    public DbSet<UnitOfMeasure> UnitOfMeasure { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Vendor> Vendor { get; set; }

    // Inventory
    public DbSet<Item> Item { get; set; }
    public DbSet<ItemStock> ItemStock { get; set; }

    // Transactions
    public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItem { get; set; }
    public DbSet<SalesOrder> SalesOrder { get; set; }
    public DbSet<SalesOrderItem> SalesOrderItem { get; set; }
    public DbSet<Quotation> Quotation { get; set; }
    public DbSet<QuotationItem> QuotationItem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<UserRole>().HasQueryFilter(ur => !ur.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(rt => !rt.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<UnitOfMeasure>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted);
        modelBuilder.Entity<Item>().HasQueryFilter(i => !i.IsDeleted);
        modelBuilder.Entity<ItemStock>().HasQueryFilter(i => !i.IsDeleted);
        modelBuilder.Entity<PurchaseOrder>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<PurchaseOrderItem>().HasQueryFilter(pi => !pi.IsDeleted);
        modelBuilder.Entity<SalesOrder>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<SalesOrderItem>().HasQueryFilter(si => !si.IsDeleted);
        modelBuilder.Entity<Quotation>().HasQueryFilter(q => !q.IsDeleted);
        modelBuilder.Entity<QuotationItem>().HasQueryFilter(qi => !qi.IsDeleted);
    }
}