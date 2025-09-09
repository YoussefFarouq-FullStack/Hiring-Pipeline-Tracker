using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs;

// Core business DTOs for Requisition entity
public class RequisitionDto
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
    public string Priority { get; set; } = "Medium";
    
    [MaxLength(1000)]
    public string? RequiredSkills { get; set; }
    
    [MaxLength(50)]
    public string? ExperienceLevel { get; set; }
    
    [MaxLength(50)]
    public string? JobLevel { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "Open";
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class RequisitionDetailDto : RequisitionDto
{
    public int ApplicationCount { get; set; }
    public List<ApplicationSummaryDto> Applications { get; set; } = new();
}

// Summary DTOs for use across the domain
public class RequisitionSummaryDto
{
    public int RequisitionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? JobLevel { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public bool IsDraft { get; set; }
}
