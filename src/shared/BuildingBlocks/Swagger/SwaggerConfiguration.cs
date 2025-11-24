using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BuildingBlocks.Swagger;

/// <summary>
/// Swagger configuration helper for consistent setup across services
/// </summary>
public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string serviceDescription,
        string version = "v1")
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(version, new OpenApiInfo
            {
                Version = version,
                Title = $"{serviceName} API",
                Description = serviceDescription,
                Contact = new OpenApiContact
                {
                    Name = "Company Development Team",
                    Email = "dev@company.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Add JWT Authentication to Swagger
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Enter your JWT token in the format: Bearer {your token}",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });

            // Include XML comments
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            if (assemblyName != null)
            {
                var xmlFile = $"{assemblyName}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }

            // Use full names for better schema generation
            options.CustomSchemaIds(type => type.FullName);
        });

        return services;
    }

}

