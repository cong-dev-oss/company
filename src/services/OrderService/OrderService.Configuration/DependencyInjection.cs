using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        // Add health checks
        services.AddHealthChecks();
        
        return services;
    }
}

