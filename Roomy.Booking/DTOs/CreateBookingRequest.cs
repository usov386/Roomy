namespace Roomy.Booking.DTOs;

/// <summary>
/// Request model for creating a new booking
/// </summary>
public class CreateBookingRequest
{
    /// <summary>
    /// The Id of the Hotel
    /// </summary>
    public Guid HotelId { get; set; }

    /// <summary>
    /// The ID of the room to book
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// The ID of the plan for the booking
    /// </summary>
    public int PlanId { get; set; }
}
