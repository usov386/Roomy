using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roomy.Search.Behaviors;
using Roomy.Search.Validators;

namespace Roomy.Data;

/// <summary>
/// Extension methods for configuring Entity Framework Core and repository services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Entity Framework Core context and repository services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRoomySearchServices(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<SearchAvailableRoomsRequestValidator>();
        // Register MediatR and handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<SearchAvailableRoomsRequestValidator>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }


    public static async Task SeedAsync(this IServiceProvider services)
    {
        using (var scope = services.CreateScope())
        {
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var dbContext = dbContextFactory.CreateDbContext();

            // Create database and apply migrations
            await dbContext.Database.MigrateAsync();

            // Populate seed data
            await DataSeeder.SeedAsync(dbContext);
        }

    }
}
