using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Roomy.Search.DTOs;
using Roomy.Search.Filters;
using Roomy.Search.Services;

namespace Roomy.Search.Controllers;

/// <summary>
/// API controller for managing room searches
/// </summary>
/// <remarks>
/// Constructor for the controller
/// </remarks>
/// <param name="roomSearchService">Service for searching rooms</param>
/// <param name="logger">Logger for logging</param>
[ApiController]
[Route("api/rooms")]
public class RoomsController(IRoomSearchService roomSearchService, ILogger<RoomsController> logger) : ControllerBase
{
    /// <summary>
    /// Searches for available rooms in a hotel based on specified parameters
    /// </summary>
    /// <param name="request">Search parameters</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>List of available rooms</returns>
    /// <response code="200">Successfully found available rooms</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Hotel not found</response>
    /// <response code="500">Server error</response>
    [HttpGet]
    [ValidateRequest]
    public async Task<IActionResult> SearchAvailableRooms([FromQuery] SearchAvailableRoomsRequest request, CancellationToken cancellationToken)
    {       
        logger.LogInformation("Received search request for hotel {HotelId}", request.HotelId);

        var result = await roomSearchService.SearchAvailableRoomsAsync(request, cancellationToken);
        return Ok(result);
    }
}
