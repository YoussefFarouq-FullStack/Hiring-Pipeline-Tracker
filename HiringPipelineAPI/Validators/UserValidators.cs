using FluentValidation;
using HiringPipelineCore.DTOs;

namespace HiringPipelineAPI.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(1, 50).WithMessage("First name must be between 1 and 50 characters")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("First name can only contain letters and spaces");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("Role IDs cannot be null");
        }
    }

    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email))
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.FirstName)
                .Length(1, 50).WithMessage("First name must be between 1 and 50 characters")
                .When(x => !string.IsNullOrEmpty(x.FirstName))
                .Matches("^[a-zA-Z\\s]+$").WithMessage("First name can only contain letters and spaces")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters")
                .When(x => !string.IsNullOrEmpty(x.LastName))
                .Matches("^[a-zA-Z\\s]+$").WithMessage("Last name can only contain letters and spaces")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("Role IDs cannot be null")
                .When(x => x.RoleIds != null);
        }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters")
                .MaximumLength(100).WithMessage("New password cannot exceed 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("New password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.NewPassword).WithMessage("New password and confirm password do not match");
        }
    }
}
