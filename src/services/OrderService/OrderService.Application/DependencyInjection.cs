using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // Add FluentValidation
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}


