using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public class IdentityDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>,
    UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure User entity
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Role entity
        builder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(r => r.Description).HasMaxLength(500);
            entity.Property(r => r.CreatedAt).IsRequired();
        });

        // Configure UserRole
        builder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            entity.Property(ur => ur.AssignedAt).IsRequired();
        });

        // Configure RefreshToken
        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.JwtId).IsRequired().HasMaxLength(100);
            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();
            entity.HasIndex(rt => rt.Token);
            entity.HasIndex(rt => rt.UserId);
        });
    }
}

