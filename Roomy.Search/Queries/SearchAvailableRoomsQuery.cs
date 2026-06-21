using MediatR;
using Roomy.Search.DTOs;

namespace Roomy.Search.Queries;

/// <summary>
/// Query for searching available rooms in a hotel based on specified parameters
/// </summary>
public class SearchAvailableRoomsQuery : IRequest<SearchResultResponse>
{
    /// <summary>
    /// The search request parameters
    /// </summary>
    public required SearchAvailableRoomsRequest Request { get; set; }
}
