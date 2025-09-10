using Microsoft.EntityFrameworkCore;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Entities;
using HiringPipelineInfrastructure.Data;

namespace HiringPipelineInfrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly HiringPipelineDbContext _context;

        public RoleService(HiringPipelineDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    Permissions = r.RolePermissions.Select(rp => new PermissionDto
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Resource = rp.Permission.Resource,
                        Action = rp.Permission.Action,
                        Description = rp.Permission.Description
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                Permissions = role.RolePermissions.Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Resource = rp.Permission.Resource,
                    Action = rp.Permission.Action,
                    Description = rp.Permission.Description
                }).ToList()
            };
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Add permissions to role
            if (createRoleDto.PermissionIds.Any())
            {
                var rolePermissions = createRoleDto.PermissionIds.Select(permissionId => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    AssignedAt = DateTime.UtcNow
                });

                _context.RolePermissions.AddRange(rolePermissions);
                await _context.SaveChangesAsync();
            }

            return await GetRoleByIdAsync(role.Id) ?? throw new InvalidOperationException("Failed to create role");
        }

        public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return null;

            role.Description = updateRoleDto.Description;
            role.UpdatedAt = DateTime.UtcNow;

            // Update permissions
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            if (updateRoleDto.PermissionIds.Any())
            {
                var rolePermissions = updateRoleDto.PermissionIds.Select(permissionId => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    AssignedAt = DateTime.UtcNow
                });

                _context.RolePermissions.AddRange(rolePermissions);
            }

            await _context.SaveChangesAsync();
            return await GetRoleByIdAsync(role.Id);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Resource = p.Resource,
                    Action = p.Action,
                    Description = p.Description
                })
                .ToListAsync();
        }

        public async Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto)
        {
            // Check if assignment already exists
            var existingAssignment = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == assignRoleDto.UserId && ur.RoleId == assignRoleDto.RoleId);

            if (existingAssignment != null) return false;

            var userRole = new UserRole
            {
                UserId = assignRoleDto.UserId,
                RoleId = assignRoleDto.RoleId,
                AssignedAt = DateTime.UtcNow,
                ExpiresAt = assignRoleDto.ExpiresAt
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null) return false;

            _context.UserRoles.Remove(userRole);
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

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();
        }
    }
}
