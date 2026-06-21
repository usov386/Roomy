using Microsoft.EntityFrameworkCore;
using Roomy.Data;
using Roomy.Data.Models;

namespace Roomy.Data.Repositories
{
    /// <summary>
    /// Repository for accessing and managing room data from the database.
    /// Provides methods to query and retrieve room information associated with hotels.
    /// </summary>
    public class RoomRepository(IDbContextFactory<AppDbContext> contextFactory) : IRoomRepository
    {
        /// <summary>
        /// Retrieves all rooms for a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation if needed.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of rooms belonging to the hotel.</returns>
        public async Task<List<Room>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken)
        {
            // Use DbContextFactory to ensure thread-safe database access
            using var context = contextFactory.CreateDbContext();
            return await context.Rooms
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId)
                .Include(r => r.RoomPlanLinks)
                    .ThenInclude(p => p.RoomPlan)
                        .ThenInclude(p => p.CancellationPolicy)
                .ToListAsync(cancellationToken);
        }
    }
}
