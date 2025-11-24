using Microsoft.AspNetCore.Authorization;

namespace OrderService.Security.Authorization.Requirements;

/// <summary>
/// Authorization requirement: User must own the order
/// </summary>
public class OrderOwnerRequirement : IAuthorizationRequirement
{
}

