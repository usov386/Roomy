using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roomy.Search.Filters;
using Roomy.Search.Services;
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

        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<SearchAvailableRoomsRequestValidator>();

        // Register action filter for validation
        services.AddScoped<ValidateRequestAttribute>();

        // Register search service
        services.AddScoped<IRoomSearchService, RoomSearchService>();

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
