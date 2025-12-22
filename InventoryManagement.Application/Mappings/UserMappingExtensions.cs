using InventoryManagement.Application.DTOs.Users;
using InventoryManagement.Core.Entities;

namespace InventoryManagement.Application.Mappings;

public static class UserMappingExtensions
{
    // Entity to DTO
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = (from ur in user.UserRoles
                     select new RoleDto
                     {
                         Id = ur.Role.Id,
                         RoleName = ur.Role.RoleName
                     }).ToList()
        };
    }

    // IQueryable to DTOs using LINQ query syntax
    public static IQueryable<UserDto> ToDto(this IQueryable<User> users)
    {
        return from user in users
               select new UserDto
               {
                   Id = user.Id,
                   UserName = user.UserName,
                   Email = user.Email,
                   FullName = user.FullName,
                   PhoneNumber = user.PhoneNumber,
                   IsActive = user.IsActive,
                   CreatedAt = user.CreatedAt,
                   Roles = (from ur in user.UserRoles
                            select new RoleDto
                            {
                                Id = ur.Role.Id,
                                RoleName = ur.Role.RoleName
                            }).ToList()
               };
    }

    // CreateDto to Entity
    public static User ToEntity(this CreateUserDto dto, string passwordHash)
    {
        return new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = passwordHash,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            IsActive = dto.IsActive
        };
    }

    // UpdateDto to Entity
    public static void UpdateEntity(this UpdateUserDto dto, User user)
    {
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;
    }
}