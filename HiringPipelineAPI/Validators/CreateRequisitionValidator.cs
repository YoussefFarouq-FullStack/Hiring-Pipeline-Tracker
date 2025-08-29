using FluentValidation;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Validators;

public class CreateRequisitionValidator : AbstractValidator<CreateRequisitionDto>
{
    public CreateRequisitionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required.")
            .Length(1, 100).WithMessage("Job title must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-&,\.]+$").WithMessage("Job title can only contain letters, numbers, spaces, hyphens, ampersands, commas, and periods.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .Length(1, 100).WithMessage("Department must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\-&]+$").WithMessage("Department can only contain letters, spaces, hyphens, and ampersands.");

        RuleFor(x => x.JobLevel)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.JobLevel))
            .WithMessage("Job level cannot exceed 50 characters.")
            .Must(BeValidJobLevel).When(x => !string.IsNullOrEmpty(x.JobLevel))
            .WithMessage("Job level must be one of: Entry, Junior, Mid, Senior, Lead, Principal, Director, VP, C-Level");
    }

    private static bool BeValidJobLevel(string? jobLevel)
    {
        if (string.IsNullOrEmpty(jobLevel)) return true;
        
        var validJobLevels = new[] { 
            "Entry", "Junior", "Mid", "Senior", "Lead", "Principal", "Director", "VP", "C-Level" 
        };
        return validJobLevels.Contains(jobLevel, StringComparer.OrdinalIgnoreCase);
    }
}
