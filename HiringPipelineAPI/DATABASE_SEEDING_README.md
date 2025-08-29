# 🌱 Database Seeding - Hiring Pipeline API

## Overview

This project includes comprehensive database seeding functionality that automatically populates the database with realistic test data during development. This ensures that frontend developers always have data to work with and can test various scenarios without manually creating test data.

## 🚀 Features

### ✨ Automatic Seeding
- **Startup Seeding**: Automatically seeds the database when the application starts in development mode
- **Smart Detection**: Only seeds if no data exists, preventing duplicate seeding
- **Environment Aware**: Only runs in development mode, never in production

### 📊 Rich Test Data
- **10 Realistic Candidates**: Various statuses (Applied, Interviewing, Offered, Hired, Rejected, On Hold)
- **7 Job Requisitions**: Different departments, levels, and statuses
- **10 Applications**: Realistic application scenarios with stage progression
- **Complete Stage History**: Full tracking of application progress through the hiring pipeline

### 🎯 Realistic Scenarios
- **Diverse Sources**: LinkedIn, Indeed, Company Website, Referrals, Glassdoor
- **Multiple Departments**: Engineering, Product, Design, Analytics
- **Various Job Levels**: Entry, Junior, Mid, Senior, Lead, Principal, Director
- **Realistic Timelines**: Applications with realistic dates and progression

## 🏗️ Architecture

### Core Components

#### 1. **DbInitializer** (`Data/DbInitializer.cs`)
- Main seeding orchestration class
- Coordinates seeding of all entities
- Handles error handling and logging

#### 2. **SeedData** (`Data/SeedData.cs`)
- Centralized configuration for all sample data
- Easy to modify and extend
- Structured data classes for type safety

#### 3. **SeedingConfiguration** (`Data/SeedingConfiguration.cs`)
- Configuration options for different seeding scenarios
- Support for different seeding types
- Configurable parameters

### Data Flow

```
Program.cs → DbInitializer.SeedAsync() → Individual Seeding Methods → Database
```

## 📋 Sample Data

### Candidates
| Name | Status | Source | Days Ago | Current Stage |
|------|--------|---------|-----------|---------------|
| John Smith | Applied | LinkedIn | 30 | Technical Interview |
| Sarah Johnson | Interviewing | Indeed | 28 | Onsite Interview |
| Michael Brown | Offered | Company Website | 25 | Offer |
| Emily Davis | Hired | Referral | 35 | Hired |
| David Wilson | Rejected | LinkedIn | 22 | Rejected |
| Lisa Anderson | Applied | Glassdoor | 15 | Phone Screen |
| Robert Taylor | Interviewing | Referral | 20 | Technical Interview |
| Jennifer Martinez | On Hold | LinkedIn | 18 | Applied |
| Christopher Garcia | Applied | Indeed | 12 | Applied |
| Amanda Rodriguez | Phone Screen | Company Website | 10 | Phone Screen |

### Requisitions
| Title | Department | Level | Status | Days Ago |
|-------|------------|-------|--------|-----------|
| Senior Software Engineer | Engineering | Senior | Open | 40 |
| Product Manager | Product | Mid | Open | 35 |
| UX Designer | Design | Junior | Open | 30 |
| DevOps Engineer | Engineering | Senior | On Hold | 25 |
| Data Scientist | Analytics | Mid | Closed | 45 |
| Frontend Developer | Engineering | Mid | Open | 20 |
| QA Engineer | Engineering | Junior | Open | 15 |

### Application Scenarios
- **John Smith** → Senior Software Engineer (Technical Interview)
- **Sarah Johnson** → Product Manager (Onsite Interview)
- **Michael Brown** → Senior Software Engineer (Offer)
- **Emily Davis** → UX Designer (Hired)
- **David Wilson** → Senior Software Engineer (Rejected)
- **Lisa Anderson** → Product Manager (Phone Screen)
- **Robert Taylor** → DevOps Engineer (Technical Interview)
- **Jennifer Martinez** → Data Scientist (Applied - On Hold)
- **Christopher Garcia** → Frontend Developer (Phone Screen)
- **Amanda Rodriguez** → QA Engineer (Applied)

## 🔧 Configuration

### Automatic Seeding
The seeding is automatically configured in `Program.cs`:

```csharp
if (app.Environment.IsDevelopment())
{
    // ... Swagger configuration ...

    // Seed the database with initial test data
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<HiringPipelineDbContext>();
        await DbInitializer.SeedAsync(context, isDevelopment: true);
    }
}
```

### Seeding Options
You can customize the seeding behavior by modifying the `SeedingConfiguration`:

```csharp
var config = new SeedingConfiguration
{
    EnableSeeding = true,
    ClearExistingData = false,
    SeedingType = SeedingType.Development,
    CandidateCount = 10,
    RequisitionCount = 7,
    GenerateStageHistory = true,
    UseRealisticDates = true,
    LogProgress = true
};
```

## 🚀 Usage

### 1. **Automatic Seeding (Default)**
- Simply run the application in development mode
- Database will be automatically seeded on first run
- No additional configuration required

### 2. **Manual Seeding**
```csharp
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<HiringPipelineDbContext>();
await DbInitializer.SeedAsync(context, isDevelopment: true);
```

### 3. **Custom Seeding**
```csharp
var config = new SeedingConfiguration
{
    SeedingType = SeedingType.Comprehensive,
    CandidateCount = 50,
    RequisitionCount = 20
};

// Use custom configuration for seeding
```

## 📊 Data Structure

### Candidate Data
- **Personal Info**: First Name, Last Name, Email, Phone
- **Professional**: LinkedIn URL, Source, Status
- **Timestamps**: Created At, Updated At
- **Statuses**: Applied, Interviewing, Offered, Hired, Rejected, On Hold

### Requisition Data
- **Job Details**: Title, Department, Job Level, Status
- **Description**: Detailed job description and requirements
- **Timestamps**: Created At, Updated At
- **Statuses**: Open, On Hold, Closed, Cancelled

### Application Data
- **Relationships**: Links candidates to requisitions
- **Progress**: Current stage and status
- **Timeline**: Applied date and progression tracking

### Stage History Data
- **Progression**: From stage to stage transitions
- **Tracking**: Who moved the application and when
- **Notes**: Context for each stage change

## 🎨 Customization

### Adding New Candidates
Modify `SeedData.Candidates.SampleData` in `SeedData.cs`:

```csharp
new CandidateData
{
    FirstName = "New",
    LastName = "Candidate",
    Email = "new.candidate@email.com",
    Phone = "+1-555-0111",
    LinkedInUrl = "https://linkedin.com/in/newcandidate",
    Source = "LinkedIn",
    Status = "Applied",
    DaysAgo = 5
}
```

### Adding New Requisitions
Modify `SeedData.Requisitions.SampleData` in `SeedData.cs`:

```csharp
new RequisitionData
{
    Title = "New Position",
    Department = "Engineering",
    JobLevel = "Mid",
    Status = "Open",
    Description = "Description of the new position",
    Requirements = "Requirements for the position",
    DaysAgo = 10
}
```

### Modifying Application Scenarios
Update `SeedData.ApplicationScenarios.SampleData` to create different application flows.

## 🔍 Monitoring and Debugging

### Console Output
The seeding process provides detailed console output:

```
Seeding database with initial test data...
Database seeded successfully with 10 candidates, 7 requisitions, 10 applications, and stage history.
```

### Error Handling
- Comprehensive try-catch blocks
- Detailed error messages
- Graceful failure handling

### Logging
- Progress logging during seeding
- Timing information
- Success/failure reporting

## 🚨 Important Notes

### Production Safety
- **Never runs in production**: Seeding only occurs in development mode
- **Data protection**: Existing data is never overwritten unless explicitly configured
- **Environment awareness**: Automatically detects development vs production

### Database Requirements
- **EF Core**: Requires Entity Framework Core to be properly configured
- **Migrations**: Database should be created and up-to-date
- **Connection**: Valid database connection string must be configured

### Performance
- **Efficient seeding**: Uses bulk operations where possible
- **Minimal overhead**: Only runs once per application startup
- **Fast execution**: Typically completes in under 1 second

## 🔮 Future Enhancements

### Planned Features
- **Random data generation**: Generate truly random test data
- **Bulk seeding**: Support for large datasets
- **Custom scenarios**: User-defined seeding scenarios
- **Data export**: Export seeded data for external use

### Integration Possibilities
- **Faker library**: Integration with Bogus for realistic fake data
- **CSV import**: Import seed data from CSV files
- **API seeding**: Seed data via API endpoints
- **Scheduled seeding**: Automated seeding at specific times

## 📞 Support

### Common Issues
1. **Seeding not running**: Check if you're in development mode
2. **Data not appearing**: Verify database connection and migrations
3. **Duplicate data**: Seeding only runs when no data exists

### Troubleshooting
- Check console output for error messages
- Verify database connection string
- Ensure Entity Framework is properly configured
- Check if database exists and is accessible

### Getting Help
- Review console output for detailed information
- Check database connection and permissions
- Verify Entity Framework configuration
- Review migration status

---

## 🎯 Benefits

### For Developers
- **Immediate Testing**: No need to manually create test data
- **Realistic Scenarios**: Test with data that mirrors production
- **Consistent Environment**: Same data across all development instances
- **Time Saving**: Focus on development, not data setup

### For Frontend Teams
- **Always Data Available**: Never see empty screens during development
- **Rich UI Testing**: Test with various data states and scenarios
- **Realistic Interactions**: Experience the app as end users would
- **Faster Development**: No waiting for data to be created

### For QA Teams
- **Consistent Test Data**: Same data across all test environments
- **Scenario Coverage**: Test various hiring pipeline states
- **Edge Case Testing**: Test with different data combinations
- **Regression Testing**: Reliable data for automated tests

---

**Happy Seeding! 🌱**

This comprehensive seeding system ensures your Hiring Pipeline API always has rich, realistic data for development and testing, making the development experience smooth and productive.
