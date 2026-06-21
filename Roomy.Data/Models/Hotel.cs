namespace Roomy.Data.Models;

/// <summary>
/// Represents a hotel in the system
/// </summary>
public class Hotel
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Hotel name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// City where the hotel is located
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Hotel address
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Navigation property for rooms in the hotel
    /// </summary>
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
