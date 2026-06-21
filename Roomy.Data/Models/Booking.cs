namespace Roomy.Data.Models;

/// <summary>
/// Represents a room booking
/// </summary>
public class Booking
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the booked room
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// ID of the hotel
    /// </summary>
    public Guid HotelId { get; set; }

    /// <summary>
    /// ID of the user who booked the room
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Check-in date
    /// </summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// Check-out date
    /// </summary>
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// Navigation property for the room
    /// </summary>
    public Room? Room { get; set; }

    /// <summary>
    /// Navigation property for the user
    /// </summary>
    public User? User { get; set; }
}
