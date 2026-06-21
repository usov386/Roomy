using System.ComponentModel.DataAnnotations;

namespace Roomy.Search.DTOs;

/// <summary>
/// Response with information about an available room
/// </summary>
/// <param name="RoomId">Room ID</param>
/// <param name="RoomNumber">Room number</param>
/// <param name="Type">Room type</param>
/// <param name="Capacity">Maximum capacity</param>
/// <param name="PricePerNight">Price per night</param>
public record RoomAvailableResponse(
    [Required]
    Guid RoomId,
    [Required(ErrorMessage = "Room number is required")]
    string RoomNumber,
    [Required(ErrorMessage = "Room type is required")]
    string Type,
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    int Capacity,
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    decimal PricePerNight);
