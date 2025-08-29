using FluentValidation;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Validators;

public class UpdateCandidateValidator : AbstractValidator<UpdateCandidateDto>
{
    public UpdateCandidateValidator()
    {
        RuleFor(x => x.FirstName)
            .Length(1, 50).When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("First name must be between 1 and 50 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.LastName)
            .Length(1, 50).When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Last name must be between 1 and 50 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Please provide a valid email address.")
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone number cannot exceed 20 characters.")
            .Matches(@"^[\+]?[0-9\s\-\(\)]+$").When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Please provide a valid phone number.");

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("LinkedIn URL cannot exceed 200 characters.")
            .Must(BeValidLinkedInUrl).When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("Please provide a valid LinkedIn URL.");

        RuleFor(x => x.Source)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Source))
            .WithMessage("Source cannot exceed 100 characters.");

        RuleFor(x => x.Status)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status cannot exceed 50 characters.")
            .Must(BeValidStatus).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: Applied, Interviewing, Offered, Hired, Rejected, Withdrawn");
    }

    private static bool BeValidLinkedInUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        
        return url.StartsWith("https://www.linkedin.com/", StringComparison.OrdinalIgnoreCase) ||
               url.StartsWith("http://www.linkedin.com/", StringComparison.OrdinalIgnoreCase) ||
               url.StartsWith("https://linkedin.com/", StringComparison.OrdinalIgnoreCase) ||
               url.StartsWith("http://linkedin.com/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        
        var validStatuses = new[] { "Applied", "Interviewing", "Offered", "Hired", "Rejected", "Withdrawn" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
