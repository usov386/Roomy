using Roomy.Data.Models;

namespace Roomy.Data.Repositories
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken);
    }
}