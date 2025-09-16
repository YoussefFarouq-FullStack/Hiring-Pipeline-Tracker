using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiringPipelineCore.Entities
{
    /// <summary>
    /// Represents a refresh token for JWT authentication
    /// </summary>
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public bool IsRevoked { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ReplacedByTokenId { get; set; }

        public DateTime? RevokedAt { get; set; }

        [MaxLength(500)]
        public string? RevokedByIp { get; set; }

        [MaxLength(500)]
        public string? CreatedByIp { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(ReplacedByTokenId))]
        public virtual RefreshToken? ReplacedByToken { get; set; }

        public virtual ICollection<RefreshToken> ReplacedTokens { get; set; } = new List<RefreshToken>();

        // Helper methods
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
