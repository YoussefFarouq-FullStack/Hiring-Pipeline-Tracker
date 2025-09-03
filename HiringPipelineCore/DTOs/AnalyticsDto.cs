namespace HiringPipelineCore.DTOs;

public class AnalyticsDto
{
    public CandidateCountsByStageDto CandidateCounts { get; set; } = new();
    public StageMetricsDto StageMetrics { get; set; } = new();
    public HiringVelocityDto HiringVelocity { get; set; } = new();
    public PipelineSummaryDto PipelineSummary { get; set; } = new();
}

public class CandidateCountsByStageDto
{
    public int Applied { get; set; }
    public int PhoneScreen { get; set; }
    public int TechnicalInterview { get; set; }
    public int OnsiteInterview { get; set; }
    public int ReferenceCheck { get; set; }
    public int Offer { get; set; }
    public int Hired { get; set; }
    public int Rejected { get; set; }
    public int Withdrawn { get; set; }
    public int TotalActive { get; set; }
    public int TotalClosed { get; set; }
}

public class StageMetricsDto
{
    public List<StageMetricDto> AverageTimeInStage { get; set; } = new();
    public List<StageMetricDto> StageConversionRates { get; set; } = new();
}

public class StageMetricDto
{
    public string Stage { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class HiringVelocityDto
{
    public double AverageTimeToHire { get; set; }
    public double AverageTimeToHireLast30Days { get; set; }
    public double AverageTimeToHireLast90Days { get; set; }
    public int HiredThisMonth { get; set; }
    public int HiredLastMonth { get; set; }
    public double HiringGrowthRate { get; set; }
}

public class PipelineSummaryDto
{
    public int TotalCandidates { get; set; }
    public int ActiveApplications { get; set; }
    public int HiredThisYear { get; set; }
    public double AveragePipelineEfficiency { get; set; }
    public List<TopPerformingRequisitionDto> TopPerformingRequisitions { get; set; } = new();
}

public class TopPerformingRequisitionDto
{
    public int RequisitionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int HiredCount { get; set; }
    public double ConversionRate { get; set; }
    public double AverageTimeToHire { get; set; }
}
