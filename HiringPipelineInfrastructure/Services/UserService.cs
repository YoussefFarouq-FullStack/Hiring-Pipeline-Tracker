using Microsoft.EntityFrameworkCore;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Entities;
using HiringPipelineInfrastructure.Data;
using BCrypt.Net;

namespace HiringPipelineInfrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly HiringPipelineDbContext _context;

        public UserService(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.IsActive)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync();
        }

        public async Task<UserDetailDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList(),
                UserRoles = user.UserRoles.Select(ur => new UserRoleDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name,
                    AssignedAt = ur.AssignedAt,
                    ExpiresAt = ur.ExpiresAt
                }).ToList(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }

        public async Task<UserDetailDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList(),
                UserRoles = user.UserRoles.Select(ur => new UserRoleDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name,
                    AssignedAt = ur.AssignedAt,
                    ExpiresAt = ur.ExpiresAt
                }).ToList(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }

        public async Task<UserDetailDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == createUserDto.Username || u.Email == createUserDto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Username or email already exists");
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign roles if provided
            if (createUserDto.RoleIds.Any())
            {
                var userRoles = createUserDto.RoleIds.Select(roleId => new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow
                });

                _context.UserRoles.AddRange(userRoles);
                await _context.SaveChangesAsync();
            }

            return await GetUserByIdAsync(user.Id) ?? throw new InvalidOperationException("Failed to create user");
        }

        public async Task<UserDetailDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            // Update fields if provided
            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                // Check if email is already taken by another user
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == updateUserDto.Email && u.Id != id);
                
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }
                
                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;

            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;

            if (updateUserDto.IsActive.HasValue)
                user.IsActive = updateUserDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            // Update roles if provided
            if (updateUserDto.RoleIds != null)
            {
                _context.UserRoles.RemoveRange(user.UserRoles);

                if (updateUserDto.RoleIds.Any())
                {
                    var userRoles = updateUserDto.RoleIds.Select(roleId => new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId,
                        AssignedAt = DateTime.UtcNow
                    });

                    _context.UserRoles.AddRange(userRoles);
                }
            }

            await _context.SaveChangesAsync();
            return await GetUserByIdAsync(user.Id);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // Soft delete by deactivating
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserRoleDto>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => new UserRoleDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name,
                    AssignedAt = ur.AssignedAt,
                    ExpiresAt = ur.ExpiresAt
                })
                .ToListAsync();
        }

        public async Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            // Get existing roles
            var existingRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            // Add new roles that don't already exist
            var newRoleIds = roleIds.Except(existingRoles);
            var userRoles = newRoleIds.Select(roleId => new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });

            _context.UserRoles.AddRange(userRoles);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRolesFromUserAsync(int userId, List<int> roleIds)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId))
                .ToListAsync();

            if (!userRoles.Any()) return false;

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
