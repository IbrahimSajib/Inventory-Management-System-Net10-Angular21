using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendor");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedOnAdd();

        builder.Property(v => v.VendorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Email)
            .HasMaxLength(200);

        builder.Property(v => v.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(v => v.Address)
            .HasMaxLength(500);

        builder.Property(v => v.City)
            .HasMaxLength(100);

        builder.Property(v => v.Country)
            .HasMaxLength(100);

        builder.Property(v => v.PostalCode)
            .HasMaxLength(20);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(v => v.Email);
    }
}