using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Services.Interfaces;

public interface IAnalyticsApiService
{
    Task<AnalyticsDto> GetDashboardAnalyticsAsync();
    Task<CandidateCountsByStageDto> GetCandidateCountsByStageAsync();
    Task<List<StageMetricDto>> GetAverageTimeInStageAsync();
    Task<List<StageMetricDto>> GetStageConversionRatesAsync();
    Task<HiringVelocityDto> GetHiringVelocityAsync();
    Task<PipelineSummaryDto> GetPipelineSummaryAsync();
    Task<List<TopPerformingRequisitionDto>> GetTopPerformingRequisitionsAsync(int limit = 5);
}
