using HiringPipelineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineAPI.Data;

public class HiringPipelineDbContext : DbContext
{
    public HiringPipelineDbContext(DbContextOptions<HiringPipelineDbContext> options)
        : base(options)
    {
    }

    public DbSet<Requisition> Requisitions { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<StageHistory> StageHistories { get; set; }
}
