using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs;

// Core business DTOs for Candidate entity

/// <summary>
/// DTO for creating a new candidate
/// </summary>
public class CreateCandidateDto
{
    /// <summary>
    /// The candidate's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's phone number (optional)
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// The original filename of the uploaded resume (optional)
    /// </summary>
    public string? ResumeFileName { get; set; }
    
    /// <summary>
    /// The server file path to the resume (optional)
    /// </summary>
    public string? ResumeFilePath { get; set; }
    
    /// <summary>
    /// Additional description or notes about the candidate (optional)
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The candidate's skills (comma-separated or one per line)
    /// </summary>
    public string? Skills { get; set; }
}

/// <summary>
/// DTO for updating an existing candidate
/// </summary>
public class UpdateCandidateDto
{
    /// <summary>
    /// The candidate's first name
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// The candidate's last name
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// The candidate's email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// The candidate's phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// The original filename of the uploaded resume
    /// </summary>
    public string? ResumeFileName { get; set; }
    
    /// <summary>
    /// The server file path to the resume
    /// </summary>
    public string? ResumeFilePath { get; set; }
    
    /// <summary>
    /// Additional description or notes about the candidate
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The candidate's skills (comma-separated or one per line)
    /// </summary>
    public string? Skills { get; set; }
    
    /// <summary>
    /// The candidate's current status
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// Core candidate data transfer object
/// </summary>
public class CandidateDto
{
    /// <summary>
    /// Unique identifier for the candidate
    /// </summary>
    public int CandidateId { get; set; }
    
    /// <summary>
    /// The candidate's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's phone number (optional)
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// The original filename of the uploaded resume (optional)
    /// </summary>
    public string? ResumeFileName { get; set; }
    
    /// <summary>
    /// The server file path to the resume (optional)
    /// </summary>
    public string? ResumeFilePath { get; set; }
    
    /// <summary>
    /// Additional description or notes about the candidate (optional)
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The candidate's skills (comma-separated or one per line)
    /// </summary>
    public string? Skills { get; set; }
    
    /// <summary>
    /// The candidate's current status
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// When the candidate record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the candidate record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Detailed candidate data transfer object with additional information
/// </summary>
public class CandidateDetailDto : CandidateDto
{
    /// <summary>
    /// Number of applications submitted by this candidate
    /// </summary>
    public int ApplicationCount { get; set; }
    
    /// <summary>
    /// List of applications submitted by this candidate
    /// </summary>
    public List<ApplicationSummaryDto> Applications { get; set; } = new();
}

/// <summary>
/// Summary candidate data transfer object for use in lists and dropdowns
/// </summary>
public class CandidateSummaryDto
{
    /// <summary>
    /// Unique identifier for the candidate
    /// </summary>
    public int CandidateId { get; set; }
    
    /// <summary>
    /// The candidate's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The candidate's current status
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
