using System.ComponentModel.DataAnnotations;

namespace Roomy.Search.DTOs;

/// <summary>
/// Request for searching available rooms
/// </summary>
/// <param name="HotelId">Hotel ID</param>
/// <param name="CheckInDate">Check-in date</param>
/// <param name="CheckOutDate">Check-out date</param>
/// <param name="NumberOfRooms">Number of rooms to book</param>
/// <param name="NumberOfAdults">Number of adult guests</param>
/// <param name="ChildrenAges">List of children ages (optional)</param>
public record SearchAvailableRoomsRequest(
    [Required(ErrorMessage = "Hotel ID is required")]
    Guid HotelId,
    [Required(ErrorMessage = "Check-in date is required")]
    DateTime CheckInDate,
    [Required(ErrorMessage = "Check-out date is required")]
    DateTime CheckOutDate,
    [Required(ErrorMessage = "Number of rooms is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be greater than 0")]
    int NumberOfRooms, // TODO: Need more clarification about this field
    [Required(ErrorMessage = "Number of adults is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of adults must be greater than 0")]
    int NumberOfAdults,
    List<int>? ChildrenAges = null);
