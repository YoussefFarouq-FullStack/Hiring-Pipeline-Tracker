namespace HiringPipelineInfrastructure.Data;

/// <summary>
/// Configuration options for database seeding
/// </summary>
public class SeedingConfiguration
{
    /// <summary>
    /// Whether to enable seeding
    /// </summary>
    public bool EnableSeeding { get; set; } = true;

    /// <summary>
    /// Whether to clear existing data before seeding
    /// </summary>
    public bool ClearExistingData { get; set; } = false;

    /// <summary>
    /// The type of data to seed
    /// </summary>
    public SeedingType SeedingType { get; set; } = SeedingType.Development;

    /// <summary>
    /// Number of candidates to generate (for random seeding)
    /// </summary>
    public int CandidateCount { get; set; } = 10;

    /// <summary>
    /// Number of requisitions to generate (for random seeding)
    /// </summary>
    public int RequisitionCount { get; set; } = 7;

    /// <summary>
    /// Whether to generate realistic stage history
    /// </summary>
    public bool GenerateStageHistory { get; set; } = true;

    /// <summary>
    /// Whether to use realistic dates (relative to current date)
    /// </summary>
    public bool UseRealisticDates { get; set; } = true;

    /// <summary>
    /// Whether to log seeding progress
    /// </summary>
    public bool LogProgress { get; set; } = true;
}

/// <summary>
/// Types of seeding available
/// </summary>
public enum SeedingType
{
    /// <summary>
    /// No seeding
    /// </summary>
    None,

    /// <summary>
    /// Development seeding with sample data
    /// </summary>
    Development,

    /// <summary>
    /// Random seeding with generated data
    /// </summary>
    Random,

    /// <summary>
    /// Minimal seeding with basic data
    /// </summary>
    Minimal,

    /// <summary>
    /// Comprehensive seeding with full dataset
    /// </summary>
    Comprehensive
}

/// <summary>
/// Seeding statistics and results
/// </summary>
public class SeedingResult
{
    /// <summary>
    /// Whether seeding was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of candidates seeded
    /// </summary>
    public int CandidatesSeeded { get; set; }

    /// <summary>
    /// Number of requisitions seeded
    /// </summary>
    public int RequisitionsSeeded { get; set; }

    /// <summary>
    /// Number of applications seeded
    /// </summary>
    public int ApplicationsSeeded { get; set; }

    /// <summary>
    /// Number of stage history entries seeded
    /// </summary>
    public int StageHistoryEntriesSeeded { get; set; }

    /// <summary>
    /// Time taken for seeding
    /// </summary>
    public TimeSpan TimeTaken { get; set; }

    /// <summary>
    /// Any error messages
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional details about the seeding process
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();
}
