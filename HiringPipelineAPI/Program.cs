using Microsoft.EntityFrameworkCore;
using HiringPipelineInfrastructure.Data;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineInfrastructure.Services;
using HiringPipelineCore.Interfaces.Repositories;
using HiringPipelineInfrastructure.Repositories;
using HiringPipelineAPI.Mappings;
using HiringPipelineAPI.Validators;
using HiringPipelineAPI.Filters;
using HiringPipelineAPI.DTOs;
using HiringPipelineCore.DTOs;
using HiringPipelineAPI.Middleware;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger Configuration with JWT Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Hiring Pipeline API", 
        Version = "v1",
        Description = "API for managing hiring pipeline with comprehensive requisition fields"
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ Register your DbContext with SQL Server
builder.Services.AddDbContext<HiringPipelineDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Register individual validators
builder.Services.AddScoped<IValidator<CreateCandidateDto>, CreateCandidateValidator>();
builder.Services.AddScoped<IValidator<UpdateCandidateDto>, UpdateCandidateValidator>();
builder.Services.AddScoped<IValidator<CreateApplicationDto>, CreateApplicationValidator>();
builder.Services.AddScoped<IValidator<UpdateApplicationDto>, UpdateApplicationValidator>();
builder.Services.AddScoped<IValidator<CreateRequisitionDto>, CreateRequisitionValidator>();
builder.Services.AddScoped<IValidator<UpdateRequisitionDto>, UpdateRequisitionValidator>();
builder.Services.AddScoped<IValidator<CreateStageHistoryDto>, CreateStageHistoryValidator>();
builder.Services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();
builder.Services.AddScoped<IValidator<UpdateUserDto>, UpdateUserValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordValidator>();

// Register repositories
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IRequisitionRepository, RequisitionRepository>();
builder.Services.AddScoped<IStageHistoryRepository, StageHistoryRepository>();

// Register core services
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IRequisitionService, RequisitionService>();
builder.Services.AddScoped<IStageHistoryService, StageHistoryService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();

// Register API services
builder.Services.AddScoped<HiringPipelineAPI.Services.Interfaces.ICandidateApiService, HiringPipelineAPI.Services.Implementations.CandidateApiService>();
builder.Services.AddScoped<HiringPipelineAPI.Services.Interfaces.IRequisitionApiService, HiringPipelineAPI.Services.Implementations.RequisitionApiService>();
builder.Services.AddScoped<HiringPipelineAPI.Services.Interfaces.IApplicationApiService, HiringPipelineAPI.Services.Implementations.ApplicationApiService>();
builder.Services.AddScoped<HiringPipelineAPI.Services.Interfaces.IStageHistoryApiService, HiringPipelineAPI.Services.Implementations.StageHistoryApiService>();
builder.Services.AddScoped<HiringPipelineAPI.Services.Interfaces.IAnalyticsApiService, HiringPipelineAPI.Services.Implementations.AnalyticsApiService>();
builder.Services.AddScoped<HiringPipelineAPI.Services.Interfaces.IFileUploadService, HiringPipelineAPI.Services.Implementations.FileUploadService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
        };
    });

builder.Services.AddAuthorization();

// ✅ CORS must be registered before building the app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed the database with initial test data
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<HiringPipelineDbContext>();
        await DbInitializer.SeedAsync(context, isDevelopment: true);
    }
}

// ✅ Global middleware
app.UseGlobalExceptionHandling();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
