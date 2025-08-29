using FluentValidation;
using HiringPipelineAPI.DTOs;

namespace HiringPipelineAPI.Validators;

public class CreateStageHistoryValidator : AbstractValidator<CreateStageHistoryDto>
{
    public CreateStageHistoryValidator()
    {
        RuleFor(x => x.ApplicationId)
            .GreaterThan(0).WithMessage("Application ID must be greater than 0.");

        RuleFor(x => x.FromStage)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.FromStage))
            .WithMessage("From stage cannot exceed 50 characters.")
            .Must(BeValidStage).When(x => !string.IsNullOrEmpty(x.FromStage))
            .WithMessage("From stage must be one of: Applied, Phone Screen, Technical Interview, Onsite Interview, Reference Check, Offer, Hired, Rejected");

        RuleFor(x => x.ToStage)
            .NotEmpty().WithMessage("To stage is required.")
            .MaximumLength(50).WithMessage("To stage cannot exceed 50 characters.")
            .Must(BeValidStage).WithMessage("To stage must be one of: Applied, Phone Screen, Technical Interview, Onsite Interview, Reference Check, Offer, Hired, Rejected");

        RuleFor(x => x.MovedBy)
            .NotEmpty().WithMessage("Moved by is required.")
            .Length(1, 100).WithMessage("Moved by must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\-\.]+$").WithMessage("Moved by can only contain letters, spaces, hyphens, and periods.");

        // Custom validation: ToStage should not be the same as FromStage
        RuleFor(x => x)
            .Must(HaveDifferentStages)
            .WithMessage("To stage must be different from from stage.");
    }

    private static bool BeValidStage(string? stage)
    {
        if (string.IsNullOrEmpty(stage)) return true;
        
        var validStages = new[] { 
            "Applied", "Phone Screen", "Technical Interview", "Onsite Interview", 
            "Reference Check", "Offer", "Hired", "Rejected" 
        };
        return validStages.Contains(stage, StringComparer.OrdinalIgnoreCase);
    }

    private static bool HaveDifferentStages(CreateStageHistoryDto dto)
    {
        if (string.IsNullOrEmpty(dto.FromStage)) return true;
        return !string.Equals(dto.FromStage, dto.ToStage, StringComparison.OrdinalIgnoreCase);
    }
}
