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
    /// Room type (e.g. "Single", "Double", "Suite", "Family")
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Maximum capacity (number of guests)
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Price per night
    /// </summary>
    public decimal PricePerNight { get; set; }

    /// <summary>
    /// Navigation property for the hotel
    /// </summary>
    public Hotel? Hotel { get; set; }

    /// <summary>
    /// Навігаційна властивість для бронювань цього номера
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
