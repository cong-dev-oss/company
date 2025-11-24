using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Security.Authorization.Handlers;
using OrderService.Security.Authorization.Policies;
using OrderService.Security.Authorization.Requirements;

namespace OrderService.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, OrderAuthorizationHandler>();

        // Configure authorization policies
        // Note: AddAuthorization should be called in Api layer with options
        // This method only registers handlers

        return services;
    }
}

