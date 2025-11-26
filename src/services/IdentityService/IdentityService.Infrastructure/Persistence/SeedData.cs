using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

/// <summary>
/// Seed initial data (roles, admin user)
/// </summary>
public static class SeedData
{
    public static async Task SeedAsync(IdentityDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Description = "Administrator role with full access",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await roleManager.RoleExistsAsync("Manager"))
        {
            await roleManager.CreateAsync(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Manager",
                Description = "Manager role with elevated permissions",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new Role
            {
                Id = Guid.NewGuid(),
                Name = "User",
                Description = "Standard user role",
                CreatedAt = DateTime.UtcNow
            });
        }

        // Seed Admin User (only if no admin exists)
        var adminEmail = "admin@company.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}






