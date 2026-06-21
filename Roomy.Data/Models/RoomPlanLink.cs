namespace Roomy.Data.Models;

/// <summary>
/// Junction table linking Room to RoomPlan in a many-to-many relationship
/// </summary>
public class RoomPlanLink
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to Room
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Foreign key to RoomPlan
    /// </summary>
    public int RoomPlanId { get; set; }

    // Navigation properties
    public Room? Room { get; set; }
    public RoomPlan? RoomPlan { get; set; }
}
