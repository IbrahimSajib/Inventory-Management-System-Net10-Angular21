using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(r => r.RoleName)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed default roles
        builder.HasData(
            new Role { Id = 1, RoleName = "Admin", Description = "System Administrator", CreatedAt = new DateTime(2025, 12, 21, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = 2, RoleName = "Manager", Description = "Inventory Manager", CreatedAt = new DateTime(2025, 12, 21, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = 3, RoleName = "User", Description = "Regular User", CreatedAt = new DateTime(2025, 1, 12, 21, 0, 0, DateTimeKind.Utc) }
        );
    }
}