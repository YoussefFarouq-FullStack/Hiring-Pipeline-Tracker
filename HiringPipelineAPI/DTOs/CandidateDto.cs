namespace HiringPipelineAPI.DTOs;

// API-specific DTOs for Candidate operations
// These are only used by controllers and not shared with Core

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
