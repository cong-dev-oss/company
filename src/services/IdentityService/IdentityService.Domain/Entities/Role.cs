using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Role entity extending IdentityRole
/// </summary>
public class Role : IdentityRole<Guid>
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

