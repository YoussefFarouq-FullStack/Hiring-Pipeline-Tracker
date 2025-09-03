using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineInfrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly HiringPipelineDbContext _context;

    public AnalyticsService(HiringPipelineDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsDto> GetDashboardAnalyticsAsync()
    {
        return new AnalyticsDto
        {
            CandidateCounts = await GetCandidateCountsByStageAsync(),
            StageMetrics = new StageMetricsDto
            {
                AverageTimeInStage = await GetAverageTimeInStageAsync(),
                StageConversionRates = await GetStageConversionRatesAsync()
            },
            HiringVelocity = await GetHiringVelocityAsync(),
            PipelineSummary = await GetPipelineSummaryAsync()
        };
    }

    public async Task<CandidateCountsByStageDto> GetCandidateCountsByStageAsync()
    {
        var applications = await _context.Applications
            .Include(a => a.Candidate)
            .Include(a => a.Requisition)
            .ToListAsync();

        var counts = new CandidateCountsByStageDto();

        foreach (var application in applications)
        {
            switch (application.CurrentStage?.ToLower())
            {
                case "applied":
                    counts.Applied++;
                    break;
                case "phone screen":
                    counts.PhoneScreen++;
                    break;
                case "technical interview":
                    counts.TechnicalInterview++;
                    break;
                case "onsite interview":
                    counts.OnsiteInterview++;
                    break;
                case "reference check":
                    counts.ReferenceCheck++;
                    break;
                case "offer":
                    counts.Offer++;
                    break;
                case "hired":
                    counts.Hired++;
                    break;
                case "rejected":
                    counts.Rejected++;
                    break;
                case "withdrawn":
                    counts.Withdrawn++;
                    break;
            }
        }

        counts.TotalActive = counts.Applied + counts.PhoneScreen + counts.TechnicalInterview + 
                           counts.OnsiteInterview + counts.ReferenceCheck + counts.Offer;
        counts.TotalClosed = counts.Hired + counts.Rejected + counts.Withdrawn;

        return counts;
    }

    public async Task<List<StageMetricDto>> GetAverageTimeInStageAsync()
    {
        var stageHistory = await _context.StageHistories
            .Include(sh => sh.Application)
            .OrderBy(sh => sh.ApplicationId)
            .ThenBy(sh => sh.MovedAt)
            .ToListAsync();

        var stageMetrics = new List<StageMetricDto>();
        var stageGroups = stageHistory.GroupBy(sh => sh.ToStage);

        foreach (var group in stageGroups)
        {
            var stage = group.Key;
            var applications = group.Select(sh => sh.ApplicationId).Distinct();
            var totalTimeInStage = 0.0;
            var count = 0;

            foreach (var applicationId in applications)
            {
                var applicationHistory = stageHistory
                    .Where(sh => sh.ApplicationId == applicationId)
                    .OrderBy(sh => sh.MovedAt)
                    .ToList();

                var stageEntry = applicationHistory.FirstOrDefault(sh => sh.ToStage == stage);
                var stageExit = applicationHistory.FirstOrDefault(sh => sh.FromStage == stage);

                if (stageEntry != null)
                {
                    var exitTime = stageExit?.MovedAt ?? DateTime.UtcNow;
                    var timeInStage = (exitTime - stageEntry.MovedAt).TotalDays;
                    totalTimeInStage += timeInStage;
                    count++;
                }
            }

            if (count > 0)
            {
                stageMetrics.Add(new StageMetricDto
                {
                    Stage = stage,
                    Value = Math.Round(totalTimeInStage / count, 1),
                    Unit = "days",
                    Count = count
                });
            }
        }

        return stageMetrics;
    }

    public async Task<List<StageMetricDto>> GetStageConversionRatesAsync()
    {
        var stageHistory = await _context.StageHistories
            .Include(sh => sh.Application)
            .ToListAsync();

        var conversionRates = new List<StageMetricDto>();
        var stageGroups = stageHistory.GroupBy(sh => sh.FromStage);

        foreach (var group in stageGroups.Where(g => !string.IsNullOrEmpty(g.Key)))
        {
            var stage = group.Key;
            var totalEntered = group.Count();
            var nextStageCount = group.Count(sh => 
                sh.ToStage != "Rejected" && sh.ToStage != "Withdrawn");

            var conversionRate = totalEntered > 0 ? (double)nextStageCount / totalEntered * 100 : 0;

            conversionRates.Add(new StageMetricDto
            {
                Stage = stage,
                Value = Math.Round(conversionRate, 1),
                Unit = "%",
                Count = totalEntered
            });
        }

        return conversionRates;
    }

    public async Task<HiringVelocityDto> GetHiringVelocityAsync()
    {
        var applications = await _context.Applications
            .Include(a => a.Candidate)
            .Include(a => a.Requisition)
            .Where(a => a.CurrentStage == "Hired")
            .ToListAsync();

        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);
        var ninetyDaysAgo = now.AddDays(-90);
        var thisMonth = new DateTime(now.Year, now.Month, 1);
        var lastMonth = thisMonth.AddMonths(-1);

        var hiredApplications = applications.ToList();
        var timeToHireValues = new List<double>();

        foreach (var application in hiredApplications)
        {
            var stageHistory = await _context.StageHistories
                .Where(sh => sh.ApplicationId == application.ApplicationId)
                .OrderBy(sh => sh.MovedAt)
                .ToListAsync();

            var firstStage = stageHistory.FirstOrDefault();
            if (firstStage != null)
            {
                var timeToHire = (application.UpdatedAt - firstStage.MovedAt).TotalDays;
                timeToHireValues.Add(timeToHire);
            }
        }

        var averageTimeToHire = timeToHireValues.Any() ? timeToHireValues.Average() : 0;
        var averageTimeToHireLast30Days = timeToHireValues
            .Where((_, i) => hiredApplications[i].UpdatedAt >= thirtyDaysAgo)
            .DefaultIfEmpty(0)
            .Average();
        var averageTimeToHireLast90Days = timeToHireValues
            .Where((_, i) => hiredApplications[i].UpdatedAt >= ninetyDaysAgo)
            .DefaultIfEmpty(0)
            .Average();

        var hiredThisMonth = applications.Count(a => a.UpdatedAt >= thisMonth);
        var hiredLastMonth = applications.Count(a => a.UpdatedAt >= lastMonth && a.UpdatedAt < thisMonth);
        var hiringGrowthRate = hiredLastMonth > 0 ? ((double)(hiredThisMonth - hiredLastMonth) / hiredLastMonth) * 100 : 0;

        return new HiringVelocityDto
        {
            AverageTimeToHire = Math.Round(averageTimeToHire, 1),
            AverageTimeToHireLast30Days = Math.Round(averageTimeToHireLast30Days, 1),
            AverageTimeToHireLast90Days = Math.Round(averageTimeToHireLast90Days, 1),
            HiredThisMonth = hiredThisMonth,
            HiredLastMonth = hiredLastMonth,
            HiringGrowthRate = Math.Round(hiringGrowthRate, 1)
        };
    }

    public async Task<PipelineSummaryDto> GetPipelineSummaryAsync()
    {
        var applications = await _context.Applications
            .Include(a => a.Candidate)
            .Include(a => a.Requisition)
            .ToListAsync();

        var totalCandidates = await _context.Candidates.CountAsync();
        var activeApplications = applications.Count(a => 
            a.CurrentStage != "Hired" && a.CurrentStage != "Rejected" && a.CurrentStage != "Withdrawn");
        var hiredThisYear = applications.Count(a => 
            a.CurrentStage == "Hired" && a.UpdatedAt.Year == DateTime.UtcNow.Year);

        // Calculate pipeline efficiency (simplified)
        var totalApplications = applications.Count;
        var completedApplications = applications.Count(a => 
            a.CurrentStage == "Hired" || a.CurrentStage == "Rejected" || a.CurrentStage == "Withdrawn");
        var averagePipelineEfficiency = totalApplications > 0 ? (double)completedApplications / totalApplications * 100 : 0;

        var topPerformingRequisitions = await GetTopPerformingRequisitionsAsync(5);

        return new PipelineSummaryDto
        {
            TotalCandidates = totalCandidates,
            ActiveApplications = activeApplications,
            HiredThisYear = hiredThisYear,
            AveragePipelineEfficiency = Math.Round(averagePipelineEfficiency, 1),
            TopPerformingRequisitions = topPerformingRequisitions
        };
    }

    public async Task<List<TopPerformingRequisitionDto>> GetTopPerformingRequisitionsAsync(int limit = 5)
    {
        var requisitions = await _context.Requisitions
            .Include(r => r.Applications)
            .ToListAsync();

        var topRequisitions = new List<TopPerformingRequisitionDto>();

        foreach (var requisition in requisitions)
        {
            var applications = requisition.Applications.ToList();
            var totalApplications = applications.Count;
            var hiredCount = applications.Count(a => a.CurrentStage == "Hired");
            var conversionRate = totalApplications > 0 ? (double)hiredCount / totalApplications * 100 : 0;

            // Calculate average time to hire for this requisition
            var hiredApplications = applications.Where(a => a.CurrentStage == "Hired").ToList();
            var timeToHireValues = new List<double>();

            foreach (var application in hiredApplications)
            {
                var stageHistory = await _context.StageHistories
                    .Where(sh => sh.ApplicationId == application.ApplicationId)
                    .OrderBy(sh => sh.MovedAt)
                    .ToListAsync();

                var firstStage = stageHistory.FirstOrDefault();
                if (firstStage != null)
                {
                    var timeToHire = (application.UpdatedAt - firstStage.MovedAt).TotalDays;
                    timeToHireValues.Add(timeToHire);
                }
            }

            var averageTimeToHire = timeToHireValues.Any() ? timeToHireValues.Average() : 0;

            topRequisitions.Add(new TopPerformingRequisitionDto
            {
                RequisitionId = requisition.RequisitionId,
                Title = requisition.Title,
                Department = requisition.Department,
                TotalApplications = totalApplications,
                HiredCount = hiredCount,
                ConversionRate = Math.Round(conversionRate, 1),
                AverageTimeToHire = Math.Round(averageTimeToHire, 1)
            });
        }

        return topRequisitions
            .OrderByDescending(r => r.ConversionRate)
            .ThenByDescending(r => r.HiredCount)
            .Take(limit)
            .ToList();
    }
}
