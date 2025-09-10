using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;

namespace HiringPipelineAPI.Controllers
{
    /// <summary>
    /// Role management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<RoleDto>), 200)]
        public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();

            return Ok(role);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="createRoleDto">Role creation data</param>
        /// <returns>Created role</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RoleDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await _roleService.CreateRoleAsync(createRoleDto);
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="updateRoleDto">Role update data</param>
        /// <returns>Updated role</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            if (role == null)
                return NotFound();

            return Ok(role);
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteRole(int id)
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        /// <returns>List of all permissions</returns>
        [HttpGet("permissions")]
        [ProducesResponseType(typeof(List<PermissionDto>), 200)]
        public async Task<ActionResult<List<PermissionDto>>> GetAllPermissions()
        {
            var permissions = await _roleService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        /// <param name="assignRoleDto">Role assignment data</param>
        /// <returns>Success status</returns>
        [HttpPost("assign")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _roleService.AssignRoleToUserAsync(assignRoleDto);
            if (!success)
                return BadRequest("Role assignment failed or already exists");

            return Ok(new { message = "Role assigned successfully" });
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleId">Role ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("users/{userId}/roles/{roleId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            var success = await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            if (!success)
                return NotFound();

            return Ok(new { message = "Role removed successfully" });
        }

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user roles</returns>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(List<UserRoleDto>), 200)]
        public async Task<ActionResult<List<UserRoleDto>>> GetUserRoles(int userId)
        {
            var userRoles = await _roleService.GetUserRolesAsync(userId);
            return Ok(userRoles);
        }

        /// <summary>
        /// Get user permissions
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user permissions</returns>
        [HttpGet("users/{userId}/permissions")]
        [ProducesResponseType(typeof(List<string>), 200)]
        public async Task<ActionResult<List<string>>> GetUserPermissions(int userId)
        {
            var permissions = await _roleService.GetUserPermissionsAsync(userId);
            return Ok(permissions);
        }
    }
}
