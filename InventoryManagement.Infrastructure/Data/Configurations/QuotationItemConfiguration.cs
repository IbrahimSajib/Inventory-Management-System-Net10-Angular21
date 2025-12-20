using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class QuotationItemConfiguration : IEntityTypeConfiguration<QuotationItem>
{
    public void Configure(EntityTypeBuilder<QuotationItem> builder)
    {
        builder.ToTable("QuotationItem");

        builder.HasKey(qi => qi.Id);

        builder.Property(qi => qi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(qi => qi.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(qi => qi.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(qi => qi.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(qi => qi.DiscountPercent)
            .HasPrecision(18, 2);

        builder.Property(qi => qi.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(qi => qi.NetAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(qi => qi.Description)
            .HasMaxLength(1000);

        builder.Property(qi => qi.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(qi => qi.Item)
            .WithMany(i => i.QuotationItems)
            .HasForeignKey(qi => qi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}