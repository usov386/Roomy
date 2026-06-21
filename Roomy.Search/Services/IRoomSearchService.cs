using Roomy.Search.DTOs;

namespace Roomy.Search.Services;

/// <summary>
/// Interface for the service that searches for available rooms
/// </summary>
public interface IRoomSearchService
{
    /// <summary>
    /// Searches for available rooms in a hotel based on specified parameters
    /// </summary>
    /// <param name="request">Search parameters</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Search result with available rooms</returns>
    Task<SearchResultResponse> SearchAvailableRoomsAsync(SearchAvailableRoomsRequest request, CancellationToken cancellationToken = default);
}
