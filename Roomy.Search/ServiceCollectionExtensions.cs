using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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

        // Register search service
        services.AddScoped<IRoomSearchService, RoomSearchService>();

        return services;
    }
}
