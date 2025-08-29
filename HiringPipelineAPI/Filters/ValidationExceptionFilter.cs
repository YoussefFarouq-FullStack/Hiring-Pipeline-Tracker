using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using HiringPipelineAPI.Models;

namespace HiringPipelineAPI.Filters;

public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            var response = new ValidationErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400,
                TraceId = context.HttpContext.TraceIdentifier,
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
            context.ExceptionHandled = true;
        }
    }
}
