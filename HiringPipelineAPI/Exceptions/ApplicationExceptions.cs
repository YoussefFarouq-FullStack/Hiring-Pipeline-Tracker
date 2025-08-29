namespace HiringPipelineAPI.Exceptions;

/// <summary>
/// Base exception for all application-specific exceptions
/// </summary>
public abstract class ApplicationException : Exception
{
    public int StatusCode { get; }

    protected ApplicationException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }

    protected ApplicationException(string message, Exception innerException, int statusCode = 400) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string message) : base(message, 404) { }
    
    public NotFoundException(string resourceType, object id) 
        : base($"{resourceType} with ID {id} was not found.", 404) { }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleViolationException : ApplicationException
{
    public BusinessRuleViolationException(string message) : base(message, 400) { }
}

/// <summary>
/// Exception thrown when there's a conflict (e.g., concurrent modification)
/// </summary>
public class ConflictException : ApplicationException
{
    public ConflictException(string message) : base(message, 409) { }
}

/// <summary>
/// Exception thrown when an operation is not allowed
/// </summary>
public class OperationNotAllowedException : ApplicationException
{
    public OperationNotAllowedException(string message) : base(message, 403) { }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : ApplicationException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation errors occurred.", 400)
    {
        Errors = errors;
    }
}
