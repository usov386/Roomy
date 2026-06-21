using Microsoft.EntityFrameworkCore;
using Roomy.Data.Models;

namespace Roomy.Data.Repositories;

/// <summary>
/// Repository for booking data access operations
/// </summary>
public class BookingRepository(IDbContextFactory<AppDbContext> contextFactory) : IBookingRepository
{
    /// <summary>
    /// Gets all bookings for a hotel
    /// </summary>
    /// <param name="hotelId">The hotel ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of bookings</returns>
    public async Task<List<Booking>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        // Use DbContextFactory to ensure thread-safe database access
        using var context = contextFactory.CreateDbContext();
        return await context.Bookings
            .Where(b => b.HotelId == hotelId)
            .ToListAsync(cancellationToken);
    }
}
