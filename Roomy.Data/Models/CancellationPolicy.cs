namespace Roomy.Data.Models;

/// <summary>
/// Represents a cancellation policy for a room plan
/// </summary>
public class CancellationPolicy
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to RoomPlan
    /// </summary>
    public int RoomPlanId { get; set; }

    /// <summary>
    /// Type of cancellation policy
    /// </summary>
    public required CancellationPolicyType Type { get; set; }

    /// <summary>
    /// Number of days until free cancellation is allowed
    /// </summary>
    public int? FreeRefundUntilDays { get; set; }

    // Navigation properties
    public RoomPlan? RoomPlan { get; set; }
}

/// <summary>
/// Types of cancellation policies
/// </summary>
public enum CancellationPolicyType
{
    /// <summary>
    /// No refund allowed
    /// </summary>
    NoRefund = 0,

    /// <summary>
    /// Free cancellation until a specific date and time
    /// </summary>
    FreeUntilDateTime = 1
}
