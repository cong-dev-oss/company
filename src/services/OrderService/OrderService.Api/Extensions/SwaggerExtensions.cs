using BuildingBlocks.Swagger;

namespace OrderService.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSwaggerDocumentation(
            configuration,
            "Order Service",
            "Order Management Service for Company Microservices"
        );
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnableFilter();
                options.EnableTryItOutByDefault();
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                options.DefaultModelsExpandDepth(-1);
            });
        }

        return app;
    }
}

