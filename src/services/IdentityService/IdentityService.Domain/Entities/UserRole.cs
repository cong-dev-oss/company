using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

/// <summary>
/// User-Role join entity
/// </summary>
public class UserRole : IdentityUserRole<Guid>
{
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}

