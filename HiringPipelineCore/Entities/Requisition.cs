using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HiringPipelineCore.Entities;

public class Requisition
{
    public int RequisitionId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Location { get; set; }
    
    [MaxLength(50)]
    public string? EmploymentType { get; set; }
    
    [MaxLength(100)]
    public string? Salary { get; set; }
    
    public bool IsDraft { get; set; } = true;
    
    [MaxLength(20)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    
    [MaxLength(1000)]
    public string? RequiredSkills { get; set; }
    
    [MaxLength(50)]
    public string? ExperienceLevel { get; set; }
    
    [MaxLength(50)]
    public string? JobLevel { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "Open"; // Open, Closed, On Hold
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [JsonIgnore]
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
