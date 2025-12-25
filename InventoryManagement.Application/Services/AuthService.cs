using InventoryManagement.Application.DTOs.Auth;
using InventoryManagement.Core.Common;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InventoryManagement.Application.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        // Find user with roles
        var user = await (from u in _unitOfWork.Users.GetQueryable()
                          where u.UserName == request.UserName
                          select u)
                         .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                         .FirstOrDefaultAsync();

        if (user == null)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse("Invalid username or password");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ApiResponse<LoginResponseDto>.FailureResponse("Invalid username or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse("User account is inactive");
        }

        // Get user roles
        var roles = (from ur in user.UserRoles
                     select ur.Role.RoleName).ToList();

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            User = new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles
            }
        };

        return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
    }

    public async Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        // Check if username already exists
        var existingUser = await (from u in _unitOfWork.Users.GetQueryable()
                                  where u.UserName == request.UserName
                                  select u).FirstOrDefaultAsync();

        if (existingUser != null)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse("Username already exists");
        }

        // Check if email already exists
        var existingEmail = await (from u in _unitOfWork.Users.GetQueryable()
                                   where u.Email == request.Email
                                   select u).FirstOrDefaultAsync();

        if (existingEmail != null)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse("Email already exists");
        }

        // Validate roles exist
        if (request.RoleIds.Any())
        {
            var rolesExist = await (from r in _unitOfWork.Roles.GetQueryable()
                                    where request.RoleIds.Contains(r.Id)
                                    select r.Id).CountAsync();

            if (rolesExist != request.RoleIds.Count)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("One or more invalid role IDs");
            }
        }
        else
        {
            // If no roles specified, assign default "User" role
            var userRole = await (from r in _unitOfWork.Roles.GetQueryable()
                                  where r.RoleName == "User"
                                  select r).FirstOrDefaultAsync();

            if (userRole != null)
            {
                request.RoleIds.Add(userRole.Id);
            }
        }

        // Create new user
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Assign roles
        var userRoles = from roleId in request.RoleIds
                        select new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId
                        };

        await _unitOfWork.UserRoles.AddRangeAsync(userRoles);
        await _unitOfWork.SaveChangesAsync();

        // Get role names
        var roles = await (from r in _unitOfWork.Roles.GetQueryable()
                           where request.RoleIds.Contains(r.Id)
                           select r.RoleName).ToListAsync();

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            User = new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles
            }
        };

        return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Registration successful");
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        // Find refresh token
        var storedToken = await (from rt in _unitOfWork.RefreshTokens.GetQueryable()
                                 where rt.Token == request.RefreshToken
                                    && !rt.IsRevoked
                                    && rt.ExpiresAt > DateTime.UtcNow
                                 select rt)
                                .Include(rt => rt.User)
                                    .ThenInclude(u => u.UserRoles)
                                        .ThenInclude(ur => ur.Role)
                                .FirstOrDefaultAsync();

        if (storedToken == null)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse("Invalid or expired refresh token");
        }

        var user = storedToken.User;

        if (!user.IsActive)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse("User account is inactive");
        }

        // Get roles 
        var roles = (from ur in user.UserRoles
                     select ur.Role.RoleName).ToList();

        // Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Revoke old refresh token
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(storedToken);

        // Save new refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            User = new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles
            }
        };

        return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed successfully");
    }

    public async Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken)
    {
        // Find and revoke token
        var storedToken = await (from rt in _unitOfWork.RefreshTokens.GetQueryable()
                                 where rt.Token == refreshToken && !rt.IsRevoked
                                 select rt).FirstOrDefaultAsync();

        if (storedToken == null)
        {
            return ApiResponse<bool>.FailureResponse("Token not found or already revoked");
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(storedToken);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Token revoked successfully");
    }
}