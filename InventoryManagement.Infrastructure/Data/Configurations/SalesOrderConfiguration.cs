using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrder");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.OrderDate)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.GrandTotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.ShippingAddress)
            .HasMaxLength(500);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(s => s.OrderNumber)
            .IsUnique();

        // Relationships
        builder.HasOne(s => s.Customer)
            .WithMany(c => c.SalesOrders)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.SalesOrderItems)
            .WithOne(si => si.SalesOrder)
            .HasForeignKey(si => si.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}