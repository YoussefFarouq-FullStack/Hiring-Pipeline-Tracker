public class ApplicationDto
{
    public int CandidateId { get; set; }
    public int RequisitionId { get; set; }
    public string CurrentStage { get; set; } = "Applied";
    public string? Status { get; set; }
}
