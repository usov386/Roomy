using System.ComponentModel.DataAnnotations;

namespace Roomy.Search.DTOs;

/// <summary>
/// Response data for a cancellation policy
/// </summary>
/// <param name="Type">Type of cancellation policy (NoRefund, FreeUntilDateTime)</param>
/// <param name="FreeRefundUntilDateTime">Date/time until which free cancellation is allowed (only for FreeUntilDateTime type)</param>
public record CancellationPolicyDto(
    string Type,
    DateTime? FreeRefundUntilDateTime = null);
