using Microsoft.AspNetCore.Mvc;
using HiringPipelineCore.DTOs;
using HiringPipelineAPI.Services.Interfaces;

namespace HiringPipelineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsApiService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsApiService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive dashboard analytics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<AnalyticsDto>> GetDashboardAnalytics()
    {
        try
        {
            var analytics = await _analyticsService.GetDashboardAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard analytics");
            return StatusCode(500, "An error occurred while retrieving analytics data");
        }
    }

    /// <summary>
    /// Get candidate counts by stage
    /// </summary>
    [HttpGet("candidate-counts")]
    public async Task<ActionResult<CandidateCountsByStageDto>> GetCandidateCountsByStage()
    {
        try
        {
            var counts = await _analyticsService.GetCandidateCountsByStageAsync();
            return Ok(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving candidate counts by stage");
            return StatusCode(500, "An error occurred while retrieving candidate counts");
        }
    }

    /// <summary>
    /// Get average time spent in each stage
    /// </summary>
    [HttpGet("average-time-in-stage")]
    public async Task<ActionResult<List<StageMetricDto>>> GetAverageTimeInStage()
    {
        try
        {
            var metrics = await _analyticsService.GetAverageTimeInStageAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving average time in stage metrics");
            return StatusCode(500, "An error occurred while retrieving stage time metrics");
        }
    }

    /// <summary>
    /// Get stage conversion rates
    /// </summary>
    [HttpGet("conversion-rates")]
    public async Task<ActionResult<List<StageMetricDto>>> GetStageConversionRates()
    {
        try
        {
            var rates = await _analyticsService.GetStageConversionRatesAsync();
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stage conversion rates");
            return StatusCode(500, "An error occurred while retrieving conversion rates");
        }
    }

    /// <summary>
    /// Get hiring velocity metrics
    /// </summary>
    [HttpGet("hiring-velocity")]
    public async Task<ActionResult<HiringVelocityDto>> GetHiringVelocity()
    {
        try
        {
            var velocity = await _analyticsService.GetHiringVelocityAsync();
            return Ok(velocity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hiring velocity metrics");
            return StatusCode(500, "An error occurred while retrieving hiring velocity data");
        }
    }

    /// <summary>
    /// Get pipeline summary
    /// </summary>
    [HttpGet("pipeline-summary")]
    public async Task<ActionResult<PipelineSummaryDto>> GetPipelineSummary()
    {
        try
        {
            var summary = await _analyticsService.GetPipelineSummaryAsync();
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pipeline summary");
            return StatusCode(500, "An error occurred while retrieving pipeline summary");
        }
    }

    /// <summary>
    /// Get top performing requisitions
    /// </summary>
    [HttpGet("top-performing-requisitions")]
    public async Task<ActionResult<List<TopPerformingRequisitionDto>>> GetTopPerformingRequisitions([FromQuery] int limit = 5)
    {
        try
        {
            if (limit <= 0 || limit > 20)
            {
                return BadRequest("Limit must be between 1 and 20");
            }

            var requisitions = await _analyticsService.GetTopPerformingRequisitionsAsync(limit);
            return Ok(requisitions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top performing requisitions");
            return StatusCode(500, "An error occurred while retrieving top performing requisitions");
        }
    }
}
