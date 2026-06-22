using System.ComponentModel.DataAnnotations;

namespace Roomy.Booking.DTOs;

/// <summary>
/// Request model for creating a new booking
/// </summary>
/// <param name="HotelId">Hotel ID</param>
/// <param name="RoomId">Room Id</param>
/// <param name="PlanId">Plan Id</param>
/// <param name="CheckInDate">Check-in date</param>
/// <param name="CheckOutDate">Check-out date</param>
public record CreateBookingRequest(
    [Required(ErrorMessage = "Hotel ID is required")]
    Guid HotelId,
    [Required(ErrorMessage = "RoomId is required")]
    Guid RoomId,
    [Required(ErrorMessage = "PlanId is required")]
    int PlanId,
    [Required(ErrorMessage = "Check-in date is required")]
    DateTime CheckInDate,
    [Required(ErrorMessage = "Check-out date is required")]
    DateTime CheckOutDate)
{    
}
