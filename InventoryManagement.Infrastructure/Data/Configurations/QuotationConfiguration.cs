using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.ToTable("Quotation");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .ValueGeneratedOnAdd();

        builder.Property(q => q.QuotationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(q => q.QuotationDate)
            .IsRequired();

        builder.Property(q => q.ValidUntil)
            .IsRequired();

        builder.Property(q => q.Status)
            .IsRequired();

        builder.Property(q => q.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(q => q.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(q => q.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(q => q.GrandTotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(q => q.Notes)
            .HasMaxLength(1000);

        builder.Property(q => q.TermsAndConditions)
            .HasMaxLength(2000);

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(q => q.QuotationNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Relationships
        builder.HasOne(q => q.Customer)
            .WithMany(c => c.Quotations)
            .HasForeignKey(q => q.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.QuotationItems)
            .WithOne(qi => qi.Quotation)
            .HasForeignKey(qi => qi.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}