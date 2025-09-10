using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public DateTime CreatedAt { get; set; }
    }

    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UserRoleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class AssignRoleDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int RoleId { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
    }

    public class CreateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        
        public List<int> PermissionIds { get; set; } = new List<int>();
    }

    public class UpdateRoleDto
    {
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
