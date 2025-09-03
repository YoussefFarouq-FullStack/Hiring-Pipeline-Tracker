using HiringPipelineCore.DTOs;
using HiringPipelineAPI.Services.Interfaces;
using HiringPipelineCore.Interfaces.Services;

namespace HiringPipelineAPI.Services.Implementations;

public class AnalyticsApiService : IAnalyticsApiService
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsApiService(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task<AnalyticsDto> GetDashboardAnalyticsAsync()
    {
        return await _analyticsService.GetDashboardAnalyticsAsync();
    }

    public async Task<CandidateCountsByStageDto> GetCandidateCountsByStageAsync()
    {
        return await _analyticsService.GetCandidateCountsByStageAsync();
    }

    public async Task<List<StageMetricDto>> GetAverageTimeInStageAsync()
    {
        return await _analyticsService.GetAverageTimeInStageAsync();
    }

    public async Task<List<StageMetricDto>> GetStageConversionRatesAsync()
    {
        return await _analyticsService.GetStageConversionRatesAsync();
    }

    public async Task<HiringVelocityDto> GetHiringVelocityAsync()
    {
        return await _analyticsService.GetHiringVelocityAsync();
    }

    public async Task<PipelineSummaryDto> GetPipelineSummaryAsync()
    {
        return await _analyticsService.GetPipelineSummaryAsync();
    }

    public async Task<List<TopPerformingRequisitionDto>> GetTopPerformingRequisitionsAsync(int limit = 5)
    {
        return await _analyticsService.GetTopPerformingRequisitionsAsync(limit);
    }
}
