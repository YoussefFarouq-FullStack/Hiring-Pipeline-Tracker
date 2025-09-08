using HiringPipelineCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace HiringPipelineInfrastructure.Data;

public class HiringPipelineDbContext : DbContext
{
    public HiringPipelineDbContext(DbContextOptions<HiringPipelineDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Requisition> Requisitions { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<StageHistory> StageHistories { get; set; }
}
