using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs;

// Core business DTOs for Requisition entity

/// <summary>
/// DTO for creating a new requisition
/// </summary>
public class CreateRequisitionDto
{
    /// <summary>
    /// The title of the job requisition
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the job requisition
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The department for this requisition
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    /// <summary>
    /// The location of the job
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }
    
    /// <summary>
    /// The type of employment (Full-time, Part-time, Contract, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? EmploymentType { get; set; }
    
    /// <summary>
    /// The salary range or amount
    /// </summary>
    [MaxLength(100)]
    public string? Salary { get; set; }
    
    /// <summary>
    /// Whether this requisition is still in draft status
    /// </summary>
    public bool IsDraft { get; set; } = true;
    
    /// <summary>
    /// The priority level of this requisition
    /// </summary>
    [MaxLength(20)]
    public string Priority { get; set; } = "Medium";
    
    /// <summary>
    /// Required skills for this position
    /// </summary>
    [MaxLength(1000)]
    public string? RequiredSkills { get; set; }
    
    /// <summary>
    /// The experience level required
    /// </summary>
    [MaxLength(50)]
    public string? ExperienceLevel { get; set; }
    
    /// <summary>
    /// The job level (Entry, Mid, Senior, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? JobLevel { get; set; }
}

/// <summary>
/// DTO for updating an existing requisition
/// </summary>
public class UpdateRequisitionDto
{
    /// <summary>
    /// The title of the job requisition
    /// </summary>
    [MaxLength(200)]
    public string? Title { get; set; }
    
    /// <summary>
    /// Detailed description of the job requisition
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The department for this requisition
    /// </summary>
    [MaxLength(100)]
    public string? Department { get; set; }
    
    /// <summary>
    /// The location of the job
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }
    
    /// <summary>
    /// The type of employment (Full-time, Part-time, Contract, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? EmploymentType { get; set; }
    
    /// <summary>
    /// The salary range or amount
    /// </summary>
    [MaxLength(100)]
    public string? Salary { get; set; }
    
    /// <summary>
    /// Whether this requisition is still in draft status
    /// </summary>
    public bool? IsDraft { get; set; }
    
    /// <summary>
    /// The priority level of this requisition
    /// </summary>
    [MaxLength(20)]
    public string? Priority { get; set; }
    
    /// <summary>
    /// Required skills for this position
    /// </summary>
    [MaxLength(1000)]
    public string? RequiredSkills { get; set; }
    
    /// <summary>
    /// The experience level required
    /// </summary>
    [MaxLength(50)]
    public string? ExperienceLevel { get; set; }
    
    /// <summary>
    /// The job level (Entry, Mid, Senior, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? JobLevel { get; set; }
    
    /// <summary>
    /// The current status of the requisition
    /// </summary>
    [MaxLength(50)]
    public string? Status { get; set; }
}

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
