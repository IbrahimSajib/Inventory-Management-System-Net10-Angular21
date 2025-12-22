namespace InventoryManagement.Application.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
}

public class RoleDto
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
}