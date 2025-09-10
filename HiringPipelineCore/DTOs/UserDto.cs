using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;
        
        public List<int> RoleIds { get; set; } = new List<int>();
    }

    public class UpdateUserDto
    {
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }
        
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string? FirstName { get; set; }
        
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }
        
        public bool? IsActive { get; set; }
        
        public List<int>? RoleIds { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters")]
        [MaxLength(100, ErrorMessage = "New password cannot exceed 100 characters")]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UserDetailDto : UserDto
    {
        public List<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();
        public new List<string> Permissions { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public new DateTime? LastLoginAt { get; set; }
    }
}
