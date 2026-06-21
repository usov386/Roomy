using System.ComponentModel.DataAnnotations;

namespace Roomy.Search.DTOs;

/// <summary>
/// Response with information about an available room
/// </summary>
/// <param name="RoomId">Room ID</param>
/// <param name="RoomNumber">Room number</param>
/// <param name="Type">Room type</param>
/// <param name="Capacity">Maximum capacity</param>
/// <param name="AvailablePlans">List of available pricing plans for this room</param>
public record RoomAvailableResponse(
    [Required(ErrorMessage = "Room number is required")]
    string RoomNumber,
    [Required(ErrorMessage = "At least one plan must be available")]
    List<RoomPlanDto> AvailablePlans);
