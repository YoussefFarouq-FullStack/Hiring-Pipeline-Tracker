namespace HiringPipelineAPI.Models;

public class Candidate
{
    public int CandidateId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = "Applied"; // Applied, Screening, Interview, TechnicalAssessment, ReferenceCheck, Offer, Hired, Rejected, Withdrawn
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
