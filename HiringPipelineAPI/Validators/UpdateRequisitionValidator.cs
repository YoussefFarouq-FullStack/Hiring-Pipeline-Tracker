using FluentValidation;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Validators;

public class UpdateRequisitionValidator : AbstractValidator<UpdateRequisitionDto>
{
    public UpdateRequisitionValidator()
    {
        RuleFor(x => x.Title)
            .Length(1, 200).When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Job title must be between 1 and 200 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-&,\.]+$").When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Job title can only contain letters, numbers, spaces, hyphens, ampersands, commas, and periods.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Department)
            .Length(1, 100).When(x => !string.IsNullOrEmpty(x.Department))
            .WithMessage("Department must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\-&]+$").When(x => !string.IsNullOrEmpty(x.Department))
            .WithMessage("Department can only contain letters, spaces, hyphens, and ampersands.");

        RuleFor(x => x.Location)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Location))
            .WithMessage("Location cannot exceed 100 characters.");

        RuleFor(x => x.EmploymentType)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.EmploymentType))
            .WithMessage("Employment type cannot exceed 50 characters.")
            .Must(BeValidEmploymentType).When(x => !string.IsNullOrEmpty(x.EmploymentType))
            .WithMessage("Employment type must be one of: Full-time, Part-time, Contract, Internship, Temporary");

        RuleFor(x => x.Salary)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Salary))
            .WithMessage("Salary cannot exceed 100 characters.");

        RuleFor(x => x.Priority)
            .Must(BeValidPriority).When(x => !string.IsNullOrEmpty(x.Priority))
            .WithMessage("Priority must be one of: Low, Medium, High");

        RuleFor(x => x.RequiredSkills)
            .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.RequiredSkills))
            .WithMessage("Required skills cannot exceed 1000 characters.");

        RuleFor(x => x.ExperienceLevel)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.ExperienceLevel))
            .WithMessage("Experience level cannot exceed 50 characters.")
            .Must(BeValidExperienceLevel).When(x => !string.IsNullOrEmpty(x.ExperienceLevel))
            .WithMessage("Experience level must be one of: Junior, Mid, Senior");

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

    private static bool BeValidEmploymentType(string? employmentType)
    {
        if (string.IsNullOrEmpty(employmentType)) return true;
        
        var validTypes = new[] { 
            "Full-time", "Part-time", "Contract", "Internship", "Temporary" 
        };
        return validTypes.Contains(employmentType, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidPriority(string? priority)
    {
        if (string.IsNullOrEmpty(priority)) return true;
        
        var validPriorities = new[] { "Low", "Medium", "High" };
        return validPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidExperienceLevel(string? experienceLevel)
    {
        if (string.IsNullOrEmpty(experienceLevel)) return true;
        
        var validLevels = new[] { "Junior", "Mid", "Senior" };
        return validLevels.Contains(experienceLevel, StringComparer.OrdinalIgnoreCase);
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
