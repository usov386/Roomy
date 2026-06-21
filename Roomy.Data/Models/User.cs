namespace Roomy.Data.Models;

/// <summary>
/// Represents a user of the booking system
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// User's email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Navigation property for user's bookings
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
