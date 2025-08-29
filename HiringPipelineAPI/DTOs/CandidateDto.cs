namespace HiringPipelineAPI.DTOs;

// Response DTOs (what the frontend receives)
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

// Create DTOs (what the frontend sends to create)
public class CreateCandidateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Source { get; set; }
}

// Update DTOs (what the frontend sends to update)
public class UpdateCandidateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Source { get; set; }
    public string? Status { get; set; }
}
