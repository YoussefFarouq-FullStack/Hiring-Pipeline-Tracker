using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

            // Generate JWT access token
            var accessToken = GenerateJwtToken(user);
            
            // Generate refresh token
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());
            
            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
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
                ExpiresAt = accessTokenExpiration.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
        {
            var token = await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || token.User == null)
                return null;

            // Token reuse check
            if (token.IsRevoked)
            {
                // Optionally revoke all tokens for this user to block attackers
                await RevokeAllRefreshTokensForUserAsync(token.UserId, ipAddress);
                return null;
            }

            if (token.IsRevoked || token.ExpiryDate <= DateTime.UtcNow || !token.User.IsActive)
                return null;

            // Revoke the old refresh token and mark it as replaced
            var newRefreshToken = await GenerateRefreshTokenAsync(token.UserId);
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplacedByTokenId = newRefreshToken.Id;

            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            // Generate new access token
            var newAccessToken = GenerateJwtToken(token.User);
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());

            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                User = new UserDto
                {
                    Id = token.User.Id,
                    Username = token.User.Username,
                    Email = token.User.Email,
                    FirstName = token.User.FirstName,
                    LastName = token.User.LastName,
                    Roles = token.User.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    Permissions = token.User.UserRoles
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .Select(rp => rp.Permission.Name)
                        .Distinct()
                        .ToList(),
                    IsActive = token.User.IsActive,
                    LastLoginAt = token.User.LastLoginAt
                },
                ExpiresAt = accessTokenExpiration.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
        }


        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress = null)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || token.IsRevoked || token.ExpiryDate <= DateTime.UtcNow)
            {
                return false;
            }

            await RevokeRefreshTokenAsync(token, ipAddress, "Token revoked");
            return true;
        }

        public async Task<bool> RevokeAllRefreshTokensForUserAsync(int userId, string? ipAddress = null)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                await RevokeRefreshTokenAsync(token, ipAddress, "All tokens revoked for user");
            }

            return true;
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(int userId)
        {
            // Revoke all existing active tokens for this user
            var oldTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in oldTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = "system"; // or pass ipAddress if available
            }

            // Generate new cryptographically secure random token
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomBytes),
                ExpiryDate = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays()),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }


        private async Task RevokeRefreshTokenAsync(RefreshToken token, string? ipAddress, string reason)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;

            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
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
                expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetAccessTokenExpirationMinutes()
        {
            return int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
        }

        private int GetRefreshTokenExpirationDays()
        {
            return int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
        }
    }
}
