namespace HiringPipelineAPI.Middleware;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the application pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
