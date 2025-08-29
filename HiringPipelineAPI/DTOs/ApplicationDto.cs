namespace HiringPipelineAPI.DTOs;

public class CreateApplicationDto
{
    public int CandidateId { get; set; }
    public int RequisitionId { get; set; }
    public string? CurrentStage { get; set; }
    public string? Status { get; set; }
}

public class UpdateApplicationDto
{
    public string? CurrentStage { get; set; }
    public string? Status { get; set; }
}
