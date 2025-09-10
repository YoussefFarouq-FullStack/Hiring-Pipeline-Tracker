using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using HiringPipelineCore.DTOs;
using HiringPipelineCore.Interfaces.Services;
using HiringPipelineCore.Entities;
using HiringPipelineInfrastructure.Data;
using BCrypt.Net;

namespace HiringPipelineInfrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly HiringPipelineDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(HiringPipelineDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Find user by username with roles and permissions
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !user.IsActive)
            {
                return null; // User not found or inactive
            }

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null; // Invalid password
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            var expirationTime = DateTime.UtcNow.AddHours(2);
            
            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    Permissions = user.UserRoles
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .Select(rp => rp.Permission.Name)
                        .Distinct()
                        .ToList(),
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt
                },
                ExpiresAt = expirationTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("IsActive", user.IsActive.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Add role claims
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            // Add permission claims
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct();
            
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
