using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;

namespace HiringPipelineAPI.Controllers
{
    /// <summary>
    /// User management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), 200)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDetailDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDetailDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="createUserDto">User creation data</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDetailDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDetailDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">User update data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDetailDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a user (soft delete - deactivates user)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="changePasswordDto">Password change data</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/change-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.ChangePasswordAsync(id, changePasswordDto);
            if (!success)
                return BadRequest(new { message = "Invalid current password or user not found" });

            return Ok(new { message = "Password changed successfully" });
        }

        /// <summary>
        /// Deactivate a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            var success = await _userService.DeactivateUserAsync(id);
            if (!success)
                return NotFound();

            return Ok(new { message = "User deactivated successfully" });
        }

        /// <summary>
        /// Activate a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ActivateUser(int id)
        {
            var success = await _userService.ActivateUserAsync(id);
            if (!success)
                return NotFound();

            return Ok(new { message = "User activated successfully" });
        }

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>List of user roles</returns>
        [HttpGet("{id}/roles")]
        [ProducesResponseType(typeof(List<UserRoleDto>), 200)]
        public async Task<ActionResult<List<UserRoleDto>>> GetUserRoles(int id)
        {
            var userRoles = await _userService.GetUserRolesAsync(id);
            return Ok(userRoles);
        }

        /// <summary>
        /// Assign roles to user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="roleIds">List of role IDs to assign</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/roles")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AssignRolesToUser(int id, [FromBody] List<int> roleIds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.AssignRolesToUserAsync(id, roleIds);
            if (!success)
                return BadRequest("Failed to assign roles or user not found");

            return Ok(new { message = "Roles assigned successfully" });
        }

        /// <summary>
        /// Remove roles from user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="roleIds">List of role IDs to remove</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}/roles")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> RemoveRolesFromUser(int id, [FromBody] List<int> roleIds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.RemoveRolesFromUserAsync(id, roleIds);
            if (!success)
                return BadRequest("Failed to remove roles or user not found");

            return Ok(new { message = "Roles removed successfully" });
        }
    }
}
