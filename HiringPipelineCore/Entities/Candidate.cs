using System.Text.Json.Serialization;

namespace HiringPipelineCore.Entities;

public class Candidate
{
    public int CandidateId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ResumeFileName { get; set; } // Original filename of uploaded resume
    public string? ResumeFilePath { get; set; } // Server file path to resume
    public string? Description { get; set; } // Candidate description/notes
    public string? Skills { get; set; } // Comma-separated skills list
    public string Status { get; set; } = "Applied"; // Applied, Screening, Interview, TechnicalAssessment, ReferenceCheck, Offer, Hired, Rejected, Withdrawn
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [JsonIgnore]
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
