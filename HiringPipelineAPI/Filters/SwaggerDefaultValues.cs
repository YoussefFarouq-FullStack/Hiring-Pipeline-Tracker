using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace HiringPipelineAPI.Filters;

/// <summary>
/// Represents the Swagger/Swashbuckle operation filter for enhanced documentation
/// </summary>
public class SwaggerDefaultValues : IOperationFilter
{
    /// <summary>
    /// Applies the filter to the specified operation using the given context.
    /// </summary>
    /// <param name="operation">The operation to apply the filter to.</param>
    /// <param name="context">The current Swagger generation context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        // Add better response descriptions for common status codes
        if (operation.Responses.ContainsKey("400"))
        {
            operation.Responses["400"].Description = "Bad Request - Validation failed or invalid input";
        }
        
        if (operation.Responses.ContainsKey("404"))
        {
            operation.Responses["404"].Description = "Not Found - The requested resource was not found";
        }
        
        if (operation.Responses.ContainsKey("500"))
        {
            operation.Responses["500"].Description = "Internal Server Error - An unexpected error occurred";
        }

        // Add operation summary from method info if available
        var methodInfo = context.MethodInfo;
        if (methodInfo != null)
        {
            // Try to get summary from XML comments or method name
            var summary = GetMethodSummary(methodInfo);
            if (!string.IsNullOrEmpty(summary))
            {
                operation.Summary = summary;
            }
        }
    }

    private static string GetMethodSummary(MethodInfo methodInfo)
    {
        // Simple summary generation based on method name
        var methodName = methodInfo.Name;
        
        return methodName switch
        {
            "Get" => "Retrieve data",
            "GetAll" => "Retrieve all items",
            "Create" => "Create new item",
            "Update" => "Update existing item",
            "Delete" => "Delete item",
            _ => methodName
        };
    }
}
