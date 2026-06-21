using Roomy.Data.Enums;

namespace Roomy.Data.Models;

/// <summary>
/// Represents a pricing plan that can be linked to multiple rooms with cancellation policy and meal information
/// </summary>
public class    RoomPlan
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Plan name (e.g. "Standard", "Flexible", "Non-refundable")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Price per night for this plan
    /// </summary>
    public decimal PricePerNight { get; set; }

    /// <summary>
    /// Meal options included in this plan
    /// </summary>
    public required MealIncluded MealIncluded { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<RoomPlanLink> RoomPlanLinks { get; set; } = new List<RoomPlanLink>();
    public CancellationPolicy? CancellationPolicy { get; set; }
}
