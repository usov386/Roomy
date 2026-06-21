using Roomy.Data.Enums;

namespace Roomy.Data.Models;

/// <summary>
/// Represents a room in a hotel
/// </summary>
public class Room
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Hotel ID this room belongs to
    /// </summary>
    public Guid HotelId { get; set; }

    /// <summary>
    /// Room number (e.g. "101", "202")
    /// </summary>
    public required string Number { get; set; }

    /// <summary>
    /// Room type (e.g. Single, Double, Suite, Family)
    /// </summary>
    public RoomType Type { get; set; }

    /// <summary>
    /// Maximum capacity (number of guests)
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Number of sub-rooms within this room (e.g., bedrooms in a suite)
    /// </summary>
    public int NumberOfSubRooms { get; set; }

    /// <summary>
    /// Navigation property for the hotel
    /// </summary>
    public Hotel? Hotel { get; set; }

    /// <summary>
    /// Navigation property for bookings of this room
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    /// <summary>
    /// Navigation property for pricing plans linked to this room via junction table
    /// </summary>
    public ICollection<RoomPlanLink> RoomPlanLinks { get; set; } = new List<RoomPlanLink>();
}
