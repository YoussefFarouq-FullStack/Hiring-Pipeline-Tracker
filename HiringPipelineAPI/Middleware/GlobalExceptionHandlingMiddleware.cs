using System.Net;
using System.Text.Json;
using HiringPipelineAPI.Models;
using HiringPipelineAPI.Exceptions;

namespace HiringPipelineAPI.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, details) = exception switch
        {
            // Handle specific custom application exceptions first
            HiringPipelineAPI.Exceptions.NotFoundException => (HttpStatusCode.NotFound, "Resource not found", exception.Message),
            HiringPipelineAPI.Exceptions.BusinessRuleViolationException => (HttpStatusCode.BadRequest, "Business rule violation", exception.Message),
            HiringPipelineAPI.Exceptions.ConflictException => (HttpStatusCode.Conflict, "Conflict occurred", exception.Message),
            HiringPipelineAPI.Exceptions.OperationNotAllowedException => (HttpStatusCode.Forbidden, "Operation not allowed", exception.Message),
            HiringPipelineAPI.Exceptions.ValidationException => (HttpStatusCode.BadRequest, "Validation failed", exception.Message),
            
            // Handle specific .NET exceptions
            ArgumentNullException => (HttpStatusCode.BadRequest, "Required argument is missing", exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation", exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied", "You are not authorized to perform this action"),
            
            // Handle specific database-related exceptions
            Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Concurrency conflict", "The resource has been modified by another user"),
            Microsoft.EntityFrameworkCore.DbUpdateException => (HttpStatusCode.BadRequest, "Database update failed", "Unable to save changes to the database"),
            
            // Handle FluentValidation exceptions
            FluentValidation.ValidationException => (HttpStatusCode.BadRequest, "Validation failed", "One or more validation errors occurred"),
            
            // Broader .NET exceptions (catch-all for argument-related issues)
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument provided", exception.Message),
            
            // Final fallback for any other exceptions
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred", 
                  _environment.IsDevelopment() ? exception.Message : "Please try again later")
        };

        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new ErrorResponse
        {
            Type = GetErrorType(statusCode),
            Title = message,
            Status = (int)statusCode,
            TraceId = context.TraceIdentifier,
            Message = details,
            Timestamp = DateTime.UtcNow
        };

        // Add additional details in development
        if (_environment.IsDevelopment())
        {
            errorResponse.Exception = exception.GetType().Name;
            errorResponse.StackTrace = exception.StackTrace;
            
            // Add validation errors if available
            if (exception is HiringPipelineAPI.Exceptions.ValidationException validationException)
            {
                errorResponse.Details = new Dictionary<string, object>
                {
                    ["ValidationErrors"] = validationException.Errors
                };
            }
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static string GetErrorType(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
        HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        HttpStatusCode.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        HttpStatusCode.UnprocessableEntity => "https://tools.ietf.org/html/rfc4918#section-11.2",
        HttpStatusCode.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        HttpStatusCode.ServiceUnavailable => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
        _ => "https://tools.ietf.org/html/rfc7231#section-6.5.1"
    };
}
