using InventoryManagement.Core.Common;

namespace InventoryManagement.Core.Entities;

public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}