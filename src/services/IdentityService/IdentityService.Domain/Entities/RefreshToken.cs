namespace IdentityService.Domain.Entities;

/// <summary>
/// Refresh token entity for JWT token refresh
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty; // JWT ID that this refresh token belongs to
    public bool IsUsed { get; set; } // If used, cannot reuse
    public bool IsRevoked { get; set; } // If revoked, cannot use
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual User User { get; set; } = null!;
}

