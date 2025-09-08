namespace HiringPipelineCore.DTOs;

// Core business DTOs for Candidate entity
public class CandidateDto
{
    public int CandidateId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CandidateDetailDto : CandidateDto
{
    public int ApplicationCount { get; set; }
    public List<ApplicationSummaryDto> Applications { get; set; } = new();
}

// Summary DTOs for use across the domain
public class CandidateSummaryDto
{
    public int CandidateId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
