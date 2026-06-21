using Microsoft.AspNetCore.Mvc;
using Roomy.Booking.DTOs;

namespace Roomy.Booking.Controllers;

/// <summary>
/// API controller for managing booking operations
/// </summary>
[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    /// <summary>
    /// Creates a new booking for a room based on the specified room and plan
    /// </summary>
    /// <param name="request">The booking creation request containing room and plan IDs</param>
    /// <returns>The created booking details</returns>
    /// <response code="200">Booking created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Room or plan not found</response>
    /// <response code="500">Server error</response>
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        // TODO: Implement booking creation logic
        throw new NotImplementedException();
    }
}
