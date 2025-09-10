using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.Entities
{
    public class RolePermission
    {
        public int Id { get; set; }
        
        [Required]
        public int RoleId { get; set; }
        
        [Required]
        public int PermissionId { get; set; }
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
}
