using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Roomy.Data.Repositories;

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
    /// <param name="connectionString">The database connection string</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRoomyDataServices(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        // Configure DbContext with LocalDB (files stored in LocalDB default location)
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Register DbContextFactory
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        services.AddTransient<IRoomRepository, RoomRepository>();
        services.AddTransient<IBookingRepository, BookingRepository>();
        services.AddTransient<IHotelRepository, HotelRepository>();

        return services;
    }
}
