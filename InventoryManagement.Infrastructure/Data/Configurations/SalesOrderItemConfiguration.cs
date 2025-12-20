using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class SalesOrderItemConfiguration : IEntityTypeConfiguration<SalesOrderItem>
{
    public void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        builder.ToTable("SalesOrderItem");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.Id)
            .ValueGeneratedOnAdd();

        builder.Property(si => si.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(si => si.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(si => si.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(si => si.DiscountPercent)
            .HasPrecision(18, 2);

        builder.Property(si => si.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(si => si.NetAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(si => si.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(si => si.Item)
            .WithMany(i => i.SalesOrderItems)
            .HasForeignKey(si => si.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}