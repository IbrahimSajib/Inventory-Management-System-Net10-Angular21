using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs.Users;

public class UpdateUserDto
{
    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [StringLength(200)]
    public string? FullName { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public List<int> RoleIds { get; set; } = new List<int>();
}