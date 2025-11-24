using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Security;
using IdentityService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger documentation
builder.Services.AddSwaggerDocumentation(builder.Configuration);

// Add Application layer
builder.Services.AddApplication();

// Add Infrastructure layer (Identity, JWT, DbContext)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure Authorization policies
builder.Services.AddAuthorization(options =>
{
    IdentityService.Security.Authorization.AuthorizationPolicies.ConfigurePolicies(options);
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? new[] { "http://localhost:3000" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwaggerDocumentation();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "IdentityService" }))
    .WithName("HealthCheck")
    .WithTags("Health");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IdentityService.Infrastructure.Persistence.IdentityDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<IdentityService.Domain.Entities.User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<IdentityService.Domain.Entities.Role>>();
    
    await IdentityService.Infrastructure.Persistence.SeedData.SeedAsync(context, userManager, roleManager);
}

app.Run();
