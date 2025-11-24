using Microsoft.AspNetCore.Authorization;
using OrderService.Security.Authorization.Requirements;

namespace OrderService.Security.Authorization.Handlers;

/// <summary>
/// Authorization handler for OrderOwnerRequirement
/// </summary>
public class OrderAuthorizationHandler : AuthorizationHandler<OrderOwnerRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrderOwnerRequirement requirement,
        Guid orderCustomerId)
    {
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;

        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
        {
            if (userId == orderCustomerId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

