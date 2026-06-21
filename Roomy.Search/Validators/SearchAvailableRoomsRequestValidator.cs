using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Roomy.Data;
using Roomy.Data.Repositories;
using Roomy.Search.DTOs;

namespace Roomy.Search.Validators;

/// <summary>
/// Validator for SearchAvailableRoomsRequest containing business logic validation rules
/// </summary>
public class SearchAvailableRoomsRequestValidator : AbstractValidator<SearchAvailableRoomsRequest>
{
    private readonly IHotelRepository hotelRepository;

    /// <summary>
    /// Constructor that defines validation rules
    /// </summary>
    public SearchAvailableRoomsRequestValidator(IHotelRepository hotelRepository)
    {
        this.hotelRepository = hotelRepository;

        // Validate HotelId is not empty
        RuleFor(x => x.HotelId)
            .NotEmpty()
            .WithMessage("Hotel ID is required")
            .MustAsync(hotelRepository.ExistsAsync)
            .WithMessage("Hotel with the specified ID not found");

        // Validate check-out date is after check-in date
        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after check-in date");

        // Validate check-out date is after check-in date
        RuleFor(x => x.CheckOutDate)
            .LessThanOrEqualTo(x => x.CheckInDate.AddMonths(1))
            .WithMessage("Check-out date must be later than the check‑in date and within one month");

        // Validate check-in date is not in the past
        RuleFor(x => x.CheckInDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Check-in date cannot be earlier than today");

        // Validate check-in date is not too far in the future (max 1 year)
        RuleFor(x => x.CheckInDate)
            .LessThanOrEqualTo(x => DateTime.Today.AddYears(1))
            .WithMessage("Check-in date is too far in the future (maximum 1 year)");

        // Validate number of rooms
        RuleFor(x => x.NumberOfRooms)
            .GreaterThan(0)
            .WithMessage("Number of rooms must be greater than 0");

        // Validate number of adults
        RuleFor(x => x.NumberOfAdults)
            .GreaterThan(0)
            .WithMessage("Number of adults must be greater than 0");

        // Validate children ages if provided
        RuleFor(x => x.ChildrenAges)
            .ForEach(rule => rule
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(18)
                .WithMessage("Children ages must be between 0 and 18"))
            .When(x => x.ChildrenAges != null && x.ChildrenAges.Count > 0);
    }
}
