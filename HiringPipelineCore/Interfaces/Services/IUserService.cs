using HiringPipelineCore.DTOs;

namespace HiringPipelineCore.Interfaces.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDetailDto?> GetUserByIdAsync(int id);
        Task<UserDetailDto?> GetUserByUsernameAsync(string username);
        Task<UserDetailDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDetailDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<List<UserRoleDto>> GetUserRolesAsync(int userId);
        Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds);
        Task<bool> RemoveRolesFromUserAsync(int userId, List<int> roleIds);
    }
}
