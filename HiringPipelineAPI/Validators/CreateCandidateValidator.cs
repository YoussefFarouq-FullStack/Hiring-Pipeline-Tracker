using FluentValidation;
using HiringPipelineCore.DTOs;

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

        RuleFor(x => x.ResumeFileName)
            .MaximumLength(ValidationConstants.Lengths.MaxFileName)
            .WithMessage($"Resume filename cannot exceed {ValidationConstants.Lengths.MaxFileName} characters.");

        RuleFor(x => x.ResumeFilePath)
            .MaximumLength(ValidationConstants.Lengths.MaxFilePath)
            .WithMessage($"Resume file path cannot exceed {ValidationConstants.Lengths.MaxFilePath} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.Lengths.MaxDescription)
            .WithMessage($"Description cannot exceed {ValidationConstants.Lengths.MaxDescription} characters.");

        RuleFor(x => x.Skills)
            .NotEmpty().WithMessage("Skills are required.")
            .MaximumLength(ValidationConstants.Lengths.MaxSkills)
            .WithMessage($"Skills cannot exceed {ValidationConstants.Lengths.MaxSkills} characters.");
    }
}
