using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Item");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.ItemCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.ReorderLevel)
            .HasPrecision(18, 2);

        builder.Property(i => i.ImageUrl)
            .HasMaxLength(500);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(i => i.ItemCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Relationships
        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.UnitOfMeasure)
            .WithMany(u => u.Items)
            .HasForeignKey(i => i.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ItemStock)
            .WithOne(s => s.Item)
            .HasForeignKey<ItemStock>(s => s.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}