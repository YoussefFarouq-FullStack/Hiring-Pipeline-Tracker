using Microsoft.EntityFrameworkCore;
using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Data;

/// <summary>
/// Database initializer for seeding test data during development
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Seeds the database with initial test data
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="isDevelopment">Whether we're in development mode</param>
    public static async Task SeedAsync(HiringPipelineDbContext context, bool isDevelopment = true)
    {
        if (!isDevelopment)
            return;

        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed data only if no data exists
            if (await context.Candidates.AnyAsync())
            {
                Console.WriteLine("Database already contains data. Skipping seeding.");
                return;
            }

            Console.WriteLine("Seeding database with initial test data...");

            // Seed candidates
            var candidates = await SeedCandidatesAsync(context);
            
            // Seed requisitions
            var requisitions = await SeedRequisitionsAsync(context);
            
            // Seed applications
            var applications = await SeedApplicationsAsync(context, candidates, requisitions);
            
            // Seed stage history
            await SeedStageHistoryAsync(context, applications);

            await context.SaveChangesAsync();
            
            Console.WriteLine($"Database seeded successfully with {candidates.Count} candidates, {requisitions.Count} requisitions, {applications.Count} applications, and stage history.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding database: {ex.Message}");
            throw;
        }
    }

    private static async Task<List<Candidate>> SeedCandidatesAsync(HiringPipelineDbContext context)
    {
        var candidates = new List<Candidate>();

        foreach (var candidateData in SeedData.Candidates.SampleData)
        {
            var candidate = new Candidate
            {
                FirstName = candidateData.FirstName,
                LastName = candidateData.LastName,
                Email = candidateData.Email,
                Phone = candidateData.Phone,
                LinkedInUrl = candidateData.LinkedInUrl,
                Source = candidateData.Source,
                Status = candidateData.Status,
                CreatedAt = DateTime.UtcNow.AddDays(-candidateData.DaysAgo),
                UpdatedAt = DateTime.UtcNow.AddDays(-candidateData.DaysAgo + 5) // Updated 5 days after creation
            };

            candidates.Add(candidate);
        }

        await context.Candidates.AddRangeAsync(candidates);
        await context.SaveChangesAsync();
        
        return candidates;
    }

    private static async Task<List<Requisition>> SeedRequisitionsAsync(HiringPipelineDbContext context)
    {
        var requisitions = new List<Requisition>();

        foreach (var requisitionData in SeedData.Requisitions.SampleData)
        {
            var requisition = new Requisition
            {
                Title = requisitionData.Title,
                Department = requisitionData.Department,
                JobLevel = requisitionData.JobLevel,
                Status = requisitionData.Status,
                CreatedAt = DateTime.UtcNow.AddDays(-requisitionData.DaysAgo),
                UpdatedAt = DateTime.UtcNow.AddDays(-requisitionData.DaysAgo)
            };

            requisitions.Add(requisition);
        }

        await context.Requisitions.AddRangeAsync(requisitions);
        await context.SaveChangesAsync();
        
        return requisitions;
    }

    private static async Task<List<Application>> SeedApplicationsAsync(HiringPipelineDbContext context, List<Candidate> candidates, List<Requisition> requisitions)
    {
        var applications = new List<Application>();

        foreach (var scenario in SeedData.ApplicationScenarios.SampleData)
        {
            if (scenario.CandidateIndex < candidates.Count && scenario.RequisitionIndex < requisitions.Count)
            {
                var candidate = candidates[scenario.CandidateIndex];
                var requisition = requisitions[scenario.RequisitionIndex];

                var application = new Application
                {
                    CandidateId = candidate.CandidateId,
                    RequisitionId = requisition.RequisitionId,
                    CurrentStage = scenario.CurrentStage,
                    Status = scenario.Status,
                    CreatedAt = candidate.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                applications.Add(application);
            }
        }

        await context.Applications.AddRangeAsync(applications);
        await context.SaveChangesAsync();
        
        return applications;
    }

    private static async Task SeedStageHistoryAsync(HiringPipelineDbContext context, List<Application> applications)
    {
        var stageHistory = new List<StageHistory>();
        var random = new Random();

        foreach (var application in applications)
        {
            var stages = GetStagesForApplication(application.CurrentStage);
            var currentDate = application.CreatedAt;

            foreach (var stage in stages)
            {
                var stageHistoryEntry = new StageHistory
                {
                    ApplicationId = application.ApplicationId,
                    FromStage = stage.FromStage,
                    ToStage = stage.ToStage,
                    MovedBy = stage.MovedBy,
                    MovedAt = currentDate
                };

                stageHistory.Add(stageHistoryEntry);
                currentDate = currentDate.AddDays(random.Next(1, 7)); // Random days between stages
            }
        }

        await context.StageHistories.AddRangeAsync(stageHistory);
        await context.SaveChangesAsync();
    }

    private static List<StageHistoryData> GetStagesForApplication(string currentStage)
    {
        var allStages = SeedData.Stages.AllStages;
        var stageHistory = new List<StageHistoryData>();
        
        var currentStageIndex = Array.IndexOf(allStages, currentStage);
        if (currentStageIndex == -1) currentStageIndex = 0;

        for (int i = 0; i <= currentStageIndex; i++)
        {
            if (i == 0)
            {
                stageHistory.Add(new StageHistoryData
                {
                    FromStage = null,
                    ToStage = allStages[i],
                    MovedBy = "System",
                    Notes = "Application submitted"
                });
            }
            else
            {
                var stageName = allStages[i];
                var stageInfo = SeedData.Stages.StageDetails.GetValueOrDefault(stageName, new StageInfo { MovedBy = "System", Notes = "Stage transition" });

                stageHistory.Add(new StageHistoryData
                {
                    FromStage = allStages[i - 1],
                    ToStage = stageName,
                    MovedBy = stageInfo.MovedBy,
                    Notes = stageInfo.Notes
                });
            }
        }

        return stageHistory;
    }

    private class StageHistoryData
    {
        public string? FromStage { get; set; }
        public string ToStage { get; set; } = string.Empty;
        public string MovedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
