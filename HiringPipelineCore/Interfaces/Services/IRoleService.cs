using HiringPipelineCore.DTOs;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRoleAsync(int id);
        Task<List<PermissionDto>> GetAllPermissionsAsync();
        Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<List<UserRoleDto>> GetUserRolesAsync(int userId);
        Task<List<string>> GetUserPermissionsAsync(int userId);
    }
}
