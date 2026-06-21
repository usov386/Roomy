namespace Roomy.Data.Enums;

/// <summary>
/// Meal options included in a room plan
/// </summary>
public enum MealIncluded
{
    /// <summary>
    /// No meals included
    /// </summary>
    None = 0,

    /// <summary>
    /// Only breakfast included
    /// </summary>
    Breakfast = 1,

    /// <summary>
    /// Breakfast and lunch included
    /// </summary>
    BreakfastLunch = 2,

    /// <summary>
    /// Breakfast, lunch, and dinner included
    /// </summary>
    BreakfastLunchDinner = 3
}
