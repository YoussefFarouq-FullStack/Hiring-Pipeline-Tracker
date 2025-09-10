using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Resource { get; set; } = string.Empty; // e.g., "Requisition", "Candidate", "Application"
        
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // e.g., "Create", "Edit", "Delete", "View"
        
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
