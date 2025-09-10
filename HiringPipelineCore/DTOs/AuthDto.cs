using System.ComponentModel.DataAnnotations;

namespace HiringPipelineCore.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new UserDto();
        public string ExpiresAt { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Permissions { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
