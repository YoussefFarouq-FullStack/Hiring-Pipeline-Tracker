namespace HiringPipelineAPI.Models;

public class Application
{
    public int ApplicationId { get; set; }

    // Foreign Keys
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public int RequisitionId { get; set; }
    public Requisition Requisition { get; set; } = null!;

    public string CurrentStage { get; set; } = "Applied";
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<StageHistory> StageHistories { get; set; } = new List<StageHistory>();
}
