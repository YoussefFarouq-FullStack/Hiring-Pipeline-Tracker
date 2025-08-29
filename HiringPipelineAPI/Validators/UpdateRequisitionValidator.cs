using FluentValidation;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Validators;

public class UpdateRequisitionValidator : AbstractValidator<UpdateRequisitionDto>
{
    public UpdateRequisitionValidator()
    {
        RuleFor(x => x.Title)
            .Length(1, 100).When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Job title must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-&,\.]+$").When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Job title can only contain letters, numbers, spaces, hyphens, ampersands, commas, and periods.");

        RuleFor(x => x.Department)
            .Length(1, 100).When(x => !string.IsNullOrEmpty(x.Department))
            .WithMessage("Department must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\-&]+$").When(x => !string.IsNullOrEmpty(x.Department))
            .WithMessage("Department can only contain letters, spaces, hyphens, and ampersands.");

        RuleFor(x => x.JobLevel)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.JobLevel))
            .WithMessage("Job level cannot exceed 50 characters.")
            .Must(BeValidJobLevel).When(x => !string.IsNullOrEmpty(x.JobLevel))
            .WithMessage("Job level must be one of: Entry, Junior, Mid, Senior, Lead, Principal, Director, VP, C-Level");

        RuleFor(x => x.Status)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status cannot exceed 50 characters.")
            .Must(BeValidStatus).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: Open, On Hold, Closed, Cancelled");
    }

    private static bool BeValidJobLevel(string? jobLevel)
    {
        if (string.IsNullOrEmpty(jobLevel)) return true;
        
        var validJobLevels = new[] { 
            "Entry", "Junior", "Mid", "Senior", "Lead", "Principal", "Director", "VP", "C-Level" 
        };
        return validJobLevels.Contains(jobLevel, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        
        var validStatuses = new[] { "Open", "On Hold", "Closed", "Cancelled" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
