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

        RuleFor(x => x.ResumeFileName)
            .MaximumLength(ValidationConstants.Lengths.MaxFileName).When(x => !string.IsNullOrEmpty(x.ResumeFileName))
            .WithMessage($"Resume filename cannot exceed {ValidationConstants.Lengths.MaxFileName} characters.");

        RuleFor(x => x.ResumeFilePath)
            .MaximumLength(ValidationConstants.Lengths.MaxFilePath).When(x => !string.IsNullOrEmpty(x.ResumeFilePath))
            .WithMessage($"Resume file path cannot exceed {ValidationConstants.Lengths.MaxFilePath} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.Lengths.MaxDescription).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage($"Description cannot exceed {ValidationConstants.Lengths.MaxDescription} characters.");

        RuleFor(x => x.Skills)
            .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Skills))
            .WithMessage("Skills cannot exceed 1000 characters.");

        RuleFor(x => x.Status)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status cannot exceed 50 characters.")
            .Must(BeValidStatus).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: Applied, Interviewing, Offered, Hired, Rejected, Withdrawn");
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        
        var validStatuses = new[] { "Applied", "Interviewing", "Offered", "Hired", "Rejected", "Withdrawn" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
