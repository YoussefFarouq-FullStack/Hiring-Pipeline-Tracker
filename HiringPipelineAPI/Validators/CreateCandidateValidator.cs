using FluentValidation;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Validators;

public class CreateCandidateValidator : AbstractValidator<CreateCandidateDto>
{
    public CreateCandidateValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(ValidationConstants.Lengths.MinName, ValidationConstants.Lengths.MaxName)
            .WithMessage($"First name must be between {ValidationConstants.Lengths.MinName} and {ValidationConstants.Lengths.MaxName} characters.")
            .Matches(ValidationConstants.RegexPatterns.Name)
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(ValidationConstants.Lengths.MinName, ValidationConstants.Lengths.MaxName)
            .WithMessage($"Last name must be between {ValidationConstants.Lengths.MinName} and {ValidationConstants.Lengths.MaxName} characters.")
            .Matches(ValidationConstants.RegexPatterns.Name)
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please provide a valid email address.")
            .MaximumLength(ValidationConstants.Lengths.MaxEmail)
            .WithMessage($"Email cannot exceed {ValidationConstants.Lengths.MaxEmail} characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(ValidationConstants.Lengths.MaxPhone)
            .WithMessage($"Phone number cannot exceed {ValidationConstants.Lengths.MaxPhone} characters.")
            .Matches(ValidationConstants.RegexPatterns.Phone).When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Please provide a valid phone number.");

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(ValidationConstants.Lengths.MaxLinkedInUrl)
            .WithMessage($"LinkedIn URL cannot exceed {ValidationConstants.Lengths.MaxLinkedInUrl} characters.")
            .Must(BeValidLinkedInUrl).When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("Please provide a valid LinkedIn URL.");

        RuleFor(x => x.Source)
            .MaximumLength(ValidationConstants.Lengths.MaxSource)
            .WithMessage($"Source cannot exceed {ValidationConstants.Lengths.MaxSource} characters.");
    }

    private static bool BeValidLinkedInUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        
        return ValidationConstants.LinkedInUrlPatterns.ValidPatterns
            .Any(pattern => url.StartsWith(pattern, StringComparison.OrdinalIgnoreCase));
    }
}
