using FluentValidation;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Validators;

public class CreateRequisitionValidator : AbstractValidator<CreateRequisitionDto>
{
    public CreateRequisitionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required.")
            .Length(1, 200).WithMessage("Job title must be between 1 and 200 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-&,\.]+$").WithMessage("Job title can only contain letters, numbers, spaces, hyphens, ampersands, commas, and periods.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .Length(1, 100).WithMessage("Department must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\-&]+$").WithMessage("Department can only contain letters, spaces, hyphens, and ampersands.");

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
            .NotEmpty().WithMessage("Priority is required.")
            .Must(BeValidPriority).WithMessage("Priority must be one of: Low, Medium, High");

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
    }

    private static bool BeValidEmploymentType(string? employmentType)
    {
        if (string.IsNullOrEmpty(employmentType)) return true;
        
        var validTypes = new[] { 
            "Full-time", "Part-time", "Contract", "Internship", "Temporary" 
        };
        return validTypes.Contains(employmentType, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidPriority(string priority)
    {
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
}
