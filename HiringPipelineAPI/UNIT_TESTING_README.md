# 🧪 Unit Testing - Hiring Pipeline API

## Overview

This project includes comprehensive unit testing for all services and repositories using modern testing practices. The testing framework leverages interfaces and dependency injection to easily mock dependencies, ensuring isolated and reliable tests.

## 🚀 Testing Stack

### **Core Testing Framework**
- **xUnit**: Modern, extensible unit testing framework for .NET
- **Moq**: Popular mocking library for .NET
- **FluentAssertions**: Human-readable assertion library
- **AutoFixture**: Automatic test data generation

### **Test Project Structure**
```
HiringPipelineAPI.Tests/
├── TestBase.cs                    # Base test class with common utilities
├── TestData/
│   └── TestDataBuilder.cs        # Test data builders for all entities
├── Services/                      # Service layer tests
│   ├── CandidateServiceTests.cs
│   ├── ApplicationServiceTests.cs
│   └── ...
├── Repositories/                  # Repository layer tests
│   ├── CandidateRepositoryTests.cs
│   └── ...
└── Mappings/                      # AutoMapper tests
    └── AutoMapperProfileTests.cs
```

## 🏗️ Architecture

### **Test Base Class**
The `TestBase` class provides common functionality for all tests:

```csharp
public abstract class TestBase
{
    protected readonly IFixture Fixture;
    protected readonly Mock<ILogger> MockLogger;

    protected TestBase()
    {
        Fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        MockLogger = new Mock<ILogger>();
    }

    // Utility methods for common assertions and test data creation
}
```

### **Test Data Builders**
Centralized test data generation with realistic values:

```csharp
public static class TestDataBuilder
{
    public static Candidate CreateCandidate(Action<Candidate>? customizer = null)
    public static Requisition CreateRequisition(Action<Requisition>? customizer = null)
    public static Application CreateApplication(int candidateId, int requisitionId, Action<Application>? customizer = null)
    public static StageHistory CreateStageHistory(int applicationId, Action<StageHistory>? customizer = null)
    // ... and more
}
```

## 🧪 Service Layer Testing

### **CandidateService Tests**
Tests the business logic layer with mocked dependencies:

```csharp
public class CandidateServiceTests : TestBase
{
    private readonly Mock<ICandidateRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<CandidateService>> _mockLogger;
    private readonly CandidateService _service;

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedCandidates()
    {
        // Arrange
        var candidates = TestDataBuilder.CreateMany<Candidate>(3);
        var candidateDtos = TestDataBuilder.CreateMany<CandidateDto>(3);

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(candidates);
        _mockMapper.Setup(m => m.Map<IEnumerable<CandidateDto>>(candidates)).Returns(candidateDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(candidateDtos);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}
```

### **Key Testing Patterns**

#### **1. Mock Setup and Verification**
```csharp
// Setup mock behavior
_mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(candidate);

// Verify mock was called correctly
_mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
```

#### **2. Exception Testing**
```csharp
[Fact]
public async Task GetByIdAsync_WithNonExistingCandidate_ShouldThrowNotFoundException()
{
    // Arrange
    _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Candidate?)null);

    // Act & Assert
    await AssertThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(id));
}
```

#### **3. Business Logic Validation**
```csharp
[Fact]
public async Task CreateAsync_WithNonExistingCandidate_ShouldThrowNotFoundException()
{
    // Arrange
    var createDto = TestDataBuilder.CreateCreateCandidateDto();
    _mockRepository.Setup(r => r.ExistsAsync(createDto.CandidateId)).ReturnsAsync(false);

    // Act & Assert
    await AssertThrowsAsync<NotFoundException>(() => _service.CreateAsync(createDto));
    _mockRepository.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Never);
}
```

## 🗄️ Repository Layer Testing

### **In-Memory Database Testing**
Repository tests use EF Core's in-memory database for realistic testing:

```csharp
public class CandidateRepositoryTests : TestBase
{
    private readonly DbContextOptions<HiringPipelineDbContext> _dbContextOptions;

    public CandidateRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HiringPipelineDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCandidates()
    {
        // Arrange
        using var context = new HiringPipelineDbContext(_dbContextOptions);
        var repository = new CandidateRepository(context, _mockLogger.Object);
        
        var candidates = TestDataBuilder.CreateMany<Candidate>(3);
        await context.Candidates.AddRangeAsync(candidates);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(candidates, options => options.Excluding(c => c.Applications));
    }
}
```

### **Database Operation Testing**
```csharp
[Fact]
public async Task AddAsync_WithValidCandidate_ShouldAddAndReturnCandidate()
{
    // Arrange
    using var context = new HiringPipelineDbContext(_dbContextOptions);
    var repository = new CandidateRepository(context, _mockLogger.Object);
    var candidate = TestDataBuilder.CreateCandidate();

    // Act
    var result = await repository.AddAsync(candidate);

    // Assert
    result.Should().NotBeNull();
    result.CandidateId.Should().BeGreaterThan(0);

    // Verify it was actually saved to the database
    var savedCandidate = await context.Candidates.FindAsync(result.CandidateId);
    savedCandidate.Should().NotBeNull();
    savedCandidate!.FirstName.Should().Be(candidate.FirstName);
}
```

## 🔄 AutoMapper Testing

### **Configuration Validation**
```csharp
[Fact]
public void AutoMapperConfiguration_ShouldBeValid()
{
    // Arrange & Act
    var configuration = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<AutoMapperProfile>();
    });

    // Assert
    configuration.AssertConfigurationIsValid();
}
```

### **Mapping Validation**
```csharp
[Fact]
public void Map_CandidateToCandidateDto_ShouldMapCorrectly()
{
    // Arrange
    var candidate = TestDataBuilder.CreateCandidate();

    // Act
    var result = _mapper.Map<CandidateDto>(candidate);

    // Assert
    result.Should().NotBeNull();
    result.CandidateId.Should().Be(candidate.CandidateId);
    result.FirstName.Should().Be(candidate.FirstName);
    result.LastName.Should().Be(candidate.LastName);
    // ... more assertions
}
```

### **Default Value Testing**
```csharp
[Fact]
public void Map_CreateCandidateDtoToCandidate_ShouldMapCorrectly()
{
    // Arrange
    var createDto = TestDataBuilder.CreateCreateCandidateDto();

    // Act
    var result = _mapper.Map<Candidate>(createDto);

    // Assert
    result.Status.Should().Be("Applied"); // Default value
    result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
}
```

## 🎯 Test Coverage

### **Service Layer Coverage**
- ✅ **CRUD Operations**: Create, Read, Update, Delete
- ✅ **Business Logic**: Validation, business rules
- ✅ **Error Handling**: Exception scenarios
- ✅ **Dependency Interaction**: Repository and mapper calls
- ✅ **Edge Cases**: Null values, empty collections

### **Repository Layer Coverage**
- ✅ **Database Operations**: Add, Update, Delete, Query
- ✅ **Data Retrieval**: Filtering, searching, pagination
- ✅ **Relationship Handling**: Include statements, navigation properties
- ✅ **Error Scenarios**: Database exceptions, constraint violations

### **AutoMapper Coverage**
- ✅ **Entity to DTO**: All mapping configurations
- ✅ **DTO to Entity**: Creation and update mappings
- ✅ **Default Values**: Automatic field population
- ✅ **Conditional Mapping**: Null value handling
- ✅ **Collection Mapping**: Lists and arrays

## 🚀 Running Tests

### **Command Line**
```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~CandidateServiceTests"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### **Visual Studio**
- **Test Explorer**: View and run tests in the Test Explorer window
- **Code Coverage**: Analyze test coverage with built-in tools
- **Debug Tests**: Set breakpoints and debug individual tests

### **VS Code**
- **C# Extension**: Run tests with the C# extension
- **Test Explorer**: Use the Test Explorer extension for better test management

## 📊 Test Data Management

### **Realistic Test Data**
```csharp
// Generate realistic candidate data
var candidate = TestDataBuilder.CreateCandidate(c => 
{
    c.FirstName = "John";
    c.LastName = "Doe";
    c.Email = "john.doe@example.com";
    c.Source = "LinkedIn";
    c.Status = "Applied";
});

// Generate multiple test entities
var candidates = TestDataBuilder.CreateMany<Candidate>(5, c => c.Status = "Applied");
var requisitions = TestDataBuilder.CreateMany<Requisition>(3, r => r.Department = "Engineering");
```

### **Custom Test Scenarios**
```csharp
[Fact]
public async Task GetByStatusAsync_WithMultipleStatuses_ShouldReturnCorrectCounts()
{
    // Arrange
    var appliedCandidates = TestDataBuilder.CreateMany<Candidate>(3, c => c.Status = "Applied");
    var interviewingCandidates = TestDataBuilder.CreateMany<Candidate>(2, c => c.Status = "Interviewing");
    var hiredCandidates = TestDataBuilder.CreateMany<Candidate>(1, c => c.Status = "Hired");
    
    // ... test implementation
}
```

## 🔧 Test Configuration

### **Project File**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="AutoFixture" Version="4.18.1" />
  </ItemGroup>
</Project>
```

### **Test Categories**
```csharp
[Trait("Category", "Unit")]
[Trait("Category", "Service")]
public class CandidateServiceTests : TestBase
{
    // Tests for service layer
}

[Trait("Category", "Unit")]
[Trait("Category", "Repository")]
public class CandidateRepositoryTests : TestBase
{
    // Tests for repository layer
}
```

## 🎯 Best Practices

### **Test Naming Convention**
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Example: GetAllAsync_WithValidData_ShouldReturnMappedResults
}
```

### **Arrange-Act-Assert Pattern**
```csharp
[Fact]
public async Task GetByIdAsync_WithExistingCandidate_ShouldReturnMappedCandidate()
{
    // Arrange - Setup test data and mocks
    var candidateId = Fixture.Create<int>();
    var candidate = TestDataBuilder.CreateCandidate(c => c.CandidateId = candidateId);
    _mockRepository.Setup(r => r.GetByIdAsync(candidateId)).ReturnsAsync(candidate);

    // Act - Execute the method under test
    var result = await _service.GetByIdAsync(candidateId);

    // Assert - Verify the results
    result.Should().NotBeNull();
    result.CandidateId.Should().Be(candidateId);
}
```

### **Mock Verification**
```csharp
// Verify the mock was called exactly once
_mockRepository.Verify(r => r.GetByIdAsync(candidateId), Times.Once);

// Verify the mock was never called
_mockRepository.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Never);

// Verify the mock was called with specific parameters
_mockRepository.Verify(r => r.UpdateAsync(It.Is<Candidate>(c => c.CandidateId == candidateId)), Times.Once);
```

## 🔍 Debugging Tests

### **Test Output**
```csharp
[Fact]
public async Task DebugTest_ShouldShowDetailedOutput()
{
    // Arrange
    var candidate = TestDataBuilder.CreateCandidate();
    
    // Act
    var result = await _service.CreateAsync(candidate);
    
    // Assert with detailed output
    result.Should().NotBeNull();
    result.Should().BeEquivalentTo(candidate, options => 
    {
        options.Excluding(c => c.CandidateId);
        options.Excluding(c => c.CreatedAt);
        options.Excluding(c => c.UpdatedAt);
        return options;
    });
}
```

### **Test Data Inspection**
```csharp
[Fact]
public async Task InspectTestData_ShouldShowGeneratedValues()
{
    // Arrange
    var candidate = TestDataBuilder.CreateCandidate();
    
    // Log the generated data for inspection
    Console.WriteLine($"Generated Candidate: {candidate.FirstName} {candidate.LastName}");
    Console.WriteLine($"Email: {candidate.Email}");
    Console.WriteLine($"Source: {candidate.Source}");
    
    // Continue with test...
}
```

## 📈 Continuous Integration

### **GitHub Actions**
```yaml
name: Unit Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

### **Azure DevOps**
```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Unit Tests'
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
    arguments: '--configuration $(buildConfiguration) --collect:"XPlat Code Coverage"'
```

## 🚨 Common Issues and Solutions

### **Mock Setup Issues**
```csharp
// Problem: Mock not returning expected value
// Solution: Ensure proper setup
_mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(candidate);

// Problem: Mock verification failing
// Solution: Use exact parameter matching
_mockRepository.Verify(r => r.GetByIdAsync(candidateId), Times.Once);
```

### **Database Context Issues**
```csharp
// Problem: Tests interfering with each other
// Solution: Use unique database names
.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())

// Problem: Data not persisting between operations
// Solution: Ensure SaveChanges is called
await context.SaveChangesAsync();
```

### **AutoMapper Issues**
```csharp
// Problem: Mapping configuration invalid
// Solution: Validate configuration
configuration.AssertConfigurationIsValid();

// Problem: Mapping not working as expected
// Solution: Check profile registration
cfg.AddProfile<AutoMapperProfile>();
```

## 🔮 Future Enhancements

### **Planned Features**
- **Integration Tests**: End-to-end API testing
- **Performance Tests**: Load and stress testing
- **Contract Tests**: API contract validation
- **Mutation Testing**: Code quality validation

### **Advanced Testing Patterns**
- **Property-Based Testing**: Generate test cases automatically
- **Snapshot Testing**: Capture and compare test outputs
- **Behavior-Driven Development**: BDD-style test specifications
- **Test Data Factories**: More sophisticated test data generation

---

## 🎯 Benefits

### **For Developers**
- **Confidence**: Know your code works as expected
- **Refactoring**: Safe to make changes with test coverage
- **Documentation**: Tests serve as living documentation
- **Debugging**: Quickly identify issues with failing tests

### **For Teams**
- **Quality Assurance**: Catch bugs before they reach production
- **Code Review**: Tests provide context for code changes
- **Onboarding**: New developers can understand code through tests
- **Maintenance**: Easier to maintain and extend existing code

### **For Business**
- **Reliability**: Fewer production bugs and issues
- **Speed**: Faster development with confidence
- **Cost**: Reduce debugging and maintenance costs
- **User Experience**: More stable and reliable application

---

**Happy Testing! 🧪**

This comprehensive testing setup ensures your Hiring Pipeline API is robust, reliable, and maintainable. Use the tests to validate your code, catch bugs early, and maintain high code quality throughout development.
