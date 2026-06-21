using System.ComponentModel.DataAnnotations;
using Roomy.Data.Models;

namespace Roomy.Search.DTOs;

/// <summary>
/// Response data for a room plan
/// </summary>
/// <param name="PlanId">Plan ID</param>
/// <param name="PlanName">Name of the plan</param>
/// <param name="TotalPriceForStay">Total price for the entire stay</param>
/// <param name="CancellationPolicy">Cancellation policy details</param>
/// <param name="MealIncluded">Meal options included</param>
public record RoomPlanDto(
    [Required]
    int PlanId,
    [Required(ErrorMessage = "Plan name is required")]
    string PlanName,
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    decimal TotalPriceForStay,
    [Required]
    CancellationPolicyDto CancellationPolicy,
    [Required(ErrorMessage = "Meal information is required")]
    string MealIncluded);
