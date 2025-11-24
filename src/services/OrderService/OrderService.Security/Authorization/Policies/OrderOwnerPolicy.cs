using Microsoft.AspNetCore.Authorization;

namespace OrderService.Security.Authorization.Policies;

/// <summary>
/// Authorization policy names
/// </summary>
public static class OrderOwnerPolicy
{
    public const string Name = "OrderOwner";
}

