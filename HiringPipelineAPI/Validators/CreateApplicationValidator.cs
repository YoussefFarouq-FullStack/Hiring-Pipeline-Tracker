using FluentValidation;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Validators;

public class CreateApplicationValidator : AbstractValidator<CreateApplicationDto>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.CandidateId)
            .GreaterThan(0).WithMessage("Candidate ID must be greater than 0.");

        RuleFor(x => x.RequisitionId)
            .GreaterThan(0).WithMessage("Requisition ID must be greater than 0.");

        RuleFor(x => x.CurrentStage)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.CurrentStage))
            .WithMessage("Current stage cannot exceed 50 characters.")
            .Must(BeValidStage).When(x => !string.IsNullOrEmpty(x.CurrentStage))
            .WithMessage("Current stage must be one of: Applied, Phone Screen, Technical Interview, Onsite Interview, Reference Check, Offer, Hired, Rejected");

        RuleFor(x => x.Status)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status cannot exceed 50 characters.")
            .Must(BeValidStatus).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: Active, On Hold, Withdrawn, Rejected, Hired");
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

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        
        var validStatuses = new[] { "Active", "On Hold", "Withdrawn", "Rejected", "Hired" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
