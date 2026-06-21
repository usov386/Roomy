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
        // Resolve project root and set DataDirectory for database file
        var projectRoot = Directory.GetCurrentDirectory();
        var dataDirectory = Path.Combine(projectRoot, "..", "Roomy.Data");
        var dataDirectoryFull = Path.GetFullPath(dataDirectory); // Convert to absolute path
        Directory.CreateDirectory(dataDirectoryFull); // Ensure directory exists
        var mdfPath = Path.Combine(dataDirectoryFull, "RoomyDb.mdf");
        AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectoryFull);

        // Configure DbContext with file-based MDF
        var baseConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        var connectionString = $"{baseConnectionString};AttachDbFilename={mdfPath}";

        // Register DbContextFactory
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}
