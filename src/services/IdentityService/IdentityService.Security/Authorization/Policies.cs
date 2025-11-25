using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Security.Authorization;

/// <summary>
/// Authorization policy names
/// </summary>
public static class Policies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireUser = "RequireUser";
    public const string RequireManager = "RequireManager";
}

/// <summary>
/// Authorization policy configuration
/// </summary>
public static class AuthorizationPolicies
{
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        // Admin policy - requires Admin role
        options.AddPolicy(Policies.RequireAdmin, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("Admin");
        });

        // User policy - requires User role
        options.AddPolicy(Policies.RequireUser, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("User", "Admin", "Manager");
        });

        // Manager policy - requires Manager or Admin role
        options.AddPolicy(Policies.RequireManager, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("Manager", "Admin");
        });
    }
}


