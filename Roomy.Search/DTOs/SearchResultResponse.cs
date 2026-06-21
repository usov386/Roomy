using System.ComponentModel.DataAnnotations;

namespace Roomy.Search.DTOs;

/// <summary>
/// Result of searching for available rooms
/// </summary>
/// <param name="AvailableRooms">List of available rooms</param>
/// <param name="TotalCount">Total count of available rooms</param>
public record SearchResultResponse(
    [Required(ErrorMessage = "Available rooms list is required")]
    List<RoomAvailableResponse> AvailableRooms,
    [Range(0, int.MaxValue, ErrorMessage = "Total count must be greater than or equal to 0")]
    int TotalCount);
