using HiringPipelineCore.DTOs;

namespace HiringPipelineCore.Interfaces.Services;

public interface IAnalyticsService
{
    Task<AnalyticsDto> GetDashboardAnalyticsAsync();
    Task<CandidateCountsByStageDto> GetCandidateCountsByStageAsync();
    Task<List<StageMetricDto>> GetAverageTimeInStageAsync();
    Task<List<StageMetricDto>> GetStageConversionRatesAsync();
    Task<HiringVelocityDto> GetHiringVelocityAsync();
    Task<PipelineSummaryDto> GetPipelineSummaryAsync();
    Task<List<TopPerformingRequisitionDto>> GetTopPerformingRequisitionsAsync(int limit = 5);
}
