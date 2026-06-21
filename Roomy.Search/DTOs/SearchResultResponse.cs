using System.ComponentModel.DataAnnotations;

namespace Roomy.Search.DTOs;

/// <summary>
/// Result of searching for available rooms
/// </summary>
/// <param name="AvailableRooms">List of available rooms</param>
/// <param name="TotalCount">Total count of available rooms</param>
public record SearchResultResponse(
    List<RoomAvailableResponse> AvailableRooms,    
    int TotalCount);
