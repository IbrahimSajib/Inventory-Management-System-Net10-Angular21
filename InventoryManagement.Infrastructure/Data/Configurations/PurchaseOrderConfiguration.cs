using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrder");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.OrderDate)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.GrandTotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(p => p.OrderNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Relationships
        builder.HasOne(p => p.Vendor)
            .WithMany(v => v.PurchaseOrders)
            .HasForeignKey(p => p.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.PurchaseOrderItems)
            .WithOne(pi => pi.PurchaseOrder)
            .HasForeignKey(pi => pi.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}