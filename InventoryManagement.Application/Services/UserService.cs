using InventoryManagement.Application.DTOs.Common;
using InventoryManagement.Application.DTOs.Users;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services;

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(PaginationParams pagination);
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto dto);
    Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<ApiResponse<bool>> DeleteUserAsync(int id);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(PaginationParams pagination)
    {
        // Base query
        var query = from user in _unitOfWork.Users.GetQueryable()
                    where !user.IsDeleted
                    select user;

        // Search filter
        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var searchTerm = pagination.SearchTerm.ToLower();
            query = from user in query
                    where user.UserName.ToLower().Contains(searchTerm) ||
                          user.Email.ToLower().Contains(searchTerm) ||
                          (user.FullName != null && user.FullName.ToLower().Contains(searchTerm))
                    select user;
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Sorting
        if (!string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            query = pagination.SortBy.ToLower() switch
            {
                "username" => pagination.SortDescending
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName),
                "email" => pagination.SortDescending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "createdat" => pagination.SortDescending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderBy(u => u.Id)
            };
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedAt);
        }

        // Include roles
        query = query.Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role);

        // Pagination
        var users = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToDto()
            .ToListAsync();

        var result = new PagedResult<UserDto>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return ApiResponse<PagedResult<UserDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await (from u in _unitOfWork.Users.GetQueryable()
                          where u.Id == id && !u.IsDeleted
                          select u)
                         .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                         .FirstOrDefaultAsync();

        if (user == null)
        {
            return ApiResponse<UserDto>.FailureResponse("User not found");
        }

        return ApiResponse<UserDto>.SuccessResponse(user.ToDto());
    }

    public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto dto)
    {
        // Check if username exists
        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
        if (existingUser != null)
        {
            return ApiResponse<UserDto>.FailureResponse("Username already exists");
        }

        // Check if email exists
        var existingEmail = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingEmail != null)
        {
            return ApiResponse<UserDto>.FailureResponse("Email already exists");
        }

        // Validate roles
        if (dto.RoleIds.Any())
        {
            var rolesExist = await (from r in _unitOfWork.Roles.GetQueryable()
                                    where dto.RoleIds.Contains(r.Id)
                                    select r.Id).CountAsync();

            if (rolesExist != dto.RoleIds.Count)
            {
                return ApiResponse<UserDto>.FailureResponse("One or more invalid role IDs");
            }
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Create user
        var user = dto.ToEntity(passwordHash);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Assign roles
        if (dto.RoleIds.Any())
        {
            var userRoles = from roleId in dto.RoleIds
                            select new UserRole
                            {
                                UserId = user.Id,
                                RoleId = roleId
                            };

            await _unitOfWork.UserRoles.AddRangeAsync(userRoles);
            await _unitOfWork.SaveChangesAsync();
        }

        // Reload user with roles
        var createdUser = await (from u in _unitOfWork.Users.GetQueryable()
                                 where u.Id == user.Id
                                 select u)
                                .Include(u => u.UserRoles)
                                    .ThenInclude(ur => ur.Role)
                                .FirstAsync();

        return ApiResponse<UserDto>.SuccessResponse(createdUser.ToDto(), "User created successfully");
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null || user.IsDeleted)
        {
            return ApiResponse<UserDto>.FailureResponse("User not found");
        }

        // Check if new username exists (excluding current user)
        if (user.UserName != dto.UserName)
        {
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (existingUser != null)
            {
                return ApiResponse<UserDto>.FailureResponse("Username already exists");
            }
        }

        // Check if new email exists (excluding current user)
        if (user.Email != dto.Email)
        {
            var existingEmail = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingEmail != null)
            {
                return ApiResponse<UserDto>.FailureResponse("Email already exists");
            }
        }

        // Validate roles
        if (dto.RoleIds.Any())
        {
            var rolesExist = await (from r in _unitOfWork.Roles.GetQueryable()
                                    where dto.RoleIds.Contains(r.Id)
                                    select r.Id).CountAsync();

            if (rolesExist != dto.RoleIds.Count)
            {
                return ApiResponse<UserDto>.FailureResponse("One or more invalid role IDs");
            }
        }

        // Update user
        dto.UpdateEntity(user);
        _unitOfWork.Users.Update(user);

        // Update roles - remove existing and add new
        var existingRoles = await (from ur in _unitOfWork.UserRoles.GetQueryable()
                                   where ur.UserId == id
                                   select ur).ToListAsync();

        _unitOfWork.UserRoles.DeleteRange(existingRoles);

        var newUserRoles = from roleId in dto.RoleIds
                           select new UserRole
                           {
                               UserId = user.Id,
                               RoleId = roleId
                           };

        await _unitOfWork.UserRoles.AddRangeAsync(newUserRoles);
        await _unitOfWork.SaveChangesAsync();

        // Reload user with roles
        var updatedUser = await (from u in _unitOfWork.Users.GetQueryable()
                                 where u.Id == id
                                 select u)
                                .Include(u => u.UserRoles)
                                    .ThenInclude(ur => ur.Role)
                                .FirstAsync();

        return ApiResponse<UserDto>.SuccessResponse(updatedUser.ToDto(), "User updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null || user.IsDeleted)
        {
            return ApiResponse<bool>.FailureResponse("User not found");
        }

        // Soft delete
        _unitOfWork.Users.SoftDelete(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
    }
}