using FluentValidation;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Validators;

public class CreateStageHistoryValidator : AbstractValidator<CreateStageHistoryDto>
{
    private static readonly string[] ValidStages = { 
        "Applied", "Phone Screen", "Technical Interview", "Onsite Interview", 
        "Reference Check", "Offer", "Hired", "Rejected", "Withdrawn" 
    };

    private static readonly Dictionary<string, int> StageOrder = new()
    {
        { "Applied", 1 },
        { "Phone Screen", 2 },
        { "Technical Interview", 3 },
        { "Onsite Interview", 4 },
        { "Reference Check", 5 },
        { "Offer", 6 },
        { "Hired", 7 },
        { "Rejected", 8 },
        { "Withdrawn", 9 }
    };

    public CreateStageHistoryValidator()
    {
        RuleFor(x => x.ApplicationId)
            .GreaterThan(0).WithMessage("Application ID must be greater than 0.");

        RuleFor(x => x.FromStage)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.FromStage))
            .WithMessage("From stage cannot exceed 50 characters.")
            .Must(BeValidStage).When(x => !string.IsNullOrEmpty(x.FromStage))
            .WithMessage("From stage must be one of: Applied, Phone Screen, Technical Interview, Onsite Interview, Reference Check, Offer, Hired, Rejected, Withdrawn");

        RuleFor(x => x.ToStage)
            .NotEmpty().WithMessage("To stage is required.")
            .MaximumLength(50).WithMessage("To stage cannot exceed 50 characters.")
            .Must(BeValidStage).WithMessage("To stage must be one of: Applied, Phone Screen, Technical Interview, Onsite Interview, Reference Check, Offer, Hired, Rejected, Withdrawn");

        RuleFor(x => x.MovedBy)
            .NotEmpty().WithMessage("Moved by is required.")
            .Length(1, 100).WithMessage("Moved by must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\-\.]+$").WithMessage("Moved by can only contain letters, spaces, hyphens, and periods.");

        // Custom validation: ToStage should not be the same as FromStage
        RuleFor(x => x)
            .Must(HaveDifferentStages)
            .WithMessage("To stage must be different from from stage.");

        // Custom validation: Prevent skipping multiple stages unless moving to Rejected/Withdrawn
        RuleFor(x => x)
            .Must(ValidateStageProgression)
            .WithMessage("Cannot skip multiple stages unless moving to Rejected or Withdrawn status.");
    }

    private static bool BeValidStage(string? stage)
    {
        if (string.IsNullOrEmpty(stage)) return true;
        return ValidStages.Contains(stage, StringComparer.OrdinalIgnoreCase);
    }

    private static bool HaveDifferentStages(CreateStageHistoryDto dto)
    {
        if (string.IsNullOrEmpty(dto.FromStage)) return true;
        return !string.Equals(dto.FromStage, dto.ToStage, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateStageProgression(CreateStageHistoryDto dto)
    {
        if (string.IsNullOrEmpty(dto.FromStage)) return true;

        var fromStage = dto.FromStage;
        var toStage = dto.ToStage;

        // Allow moving to Rejected or Withdrawn from any stage
        if (IsTerminalStage(toStage)) return true;

        // Get stage order numbers
        if (!StageOrder.TryGetValue(fromStage, out var fromOrder) || 
            !StageOrder.TryGetValue(toStage, out var toOrder))
        {
            return false;
        }

        // Allow moving forward by one stage or backward by any number of stages
        var stageDifference = toOrder - fromOrder;
        return stageDifference == 1 || stageDifference < 0;
    }

    private static bool IsTerminalStage(string stage)
    {
        return stage.Equals("Rejected", StringComparison.OrdinalIgnoreCase) || 
               stage.Equals("Withdrawn", StringComparison.OrdinalIgnoreCase);
    }
}
