using Roomy.Data.Models;

namespace Roomy.Data.Repositories;

/// <summary>
/// Repository interface for booking operations
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Gets all booked room IDs for a hotel 
    /// </summary>
    /// <param name="hotelId">The hotel ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of booked room IDs</returns>
    public Task<List<Booking>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
}
