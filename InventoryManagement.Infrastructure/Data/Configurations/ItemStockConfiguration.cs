using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class ItemStockConfiguration : IEntityTypeConfiguration<ItemStock>
{
    public void Configure(EntityTypeBuilder<ItemStock> builder)
    {
        builder.ToTable("ItemStock");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.ItemId)
            .IsRequired();

        builder.Property(s => s.QuantityOnHand)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.QuantityReserved)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.LastRestockDate)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Computed column for QuantityAvailable (not mapped to database)
        builder.Ignore(s => s.QuantityAvailable);

        // Unique index on ItemId
        builder.HasIndex(s => s.ItemId)
            .IsUnique();
    }
}