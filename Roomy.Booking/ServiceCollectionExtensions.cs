using Microsoft.Extensions.DependencyInjection;

namespace Roomy.Booking;

/// <summary>
/// Extension methods for adding Roomy.Booking services to the dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Roomy.Booking services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddRoomyBookingServices(this IServiceCollection services)
    {
        // TODO: Add Roomy.Booking services here
        return services;
    }
}
