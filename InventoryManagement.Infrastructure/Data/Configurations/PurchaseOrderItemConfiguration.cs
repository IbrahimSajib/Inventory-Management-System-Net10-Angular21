using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItem");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pi => pi.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pi => pi.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pi => pi.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pi => pi.DiscountPercent)
            .HasPrecision(18, 2);

        builder.Property(pi => pi.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(pi => pi.NetAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pi => pi.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(pi => pi.Item)
            .WithMany(i => i.PurchaseOrderItems)
            .HasForeignKey(pi => pi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}