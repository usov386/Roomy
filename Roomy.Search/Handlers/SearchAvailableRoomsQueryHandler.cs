using MediatR;
using Microsoft.Extensions.Logging;
using Roomy.Data.Enums;
using Roomy.Data.Models;
using Roomy.Data.Repositories;
using Roomy.Search.DTOs;
using Roomy.Search.Queries;

namespace Roomy.Search.Handlers;

/// <summary>
/// Handler for processing SearchAvailableRoomsQuery requests
/// </summary>
/// <remarks>
/// Constructor for the query handler
/// </remarks>
/// <param name="roomRepository">Repository for room operations</param>
/// <param name="bookingRepository">Repository for booking operations</param>
/// <param name="logger">Logger for logging</param>
public class SearchAvailableRoomsQueryHandler(
    IRoomRepository roomRepository,
    IBookingRepository bookingRepository,
    ILogger<SearchAvailableRoomsQueryHandler> logger) : IRequestHandler<SearchAvailableRoomsQuery, SearchResultResponse>
{
    /// <summary>
    /// Handles the SearchAvailableRoomsQuery and returns available rooms
    /// </summary>
    /// <param name="request">The search query parameters</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Search result with available rooms</returns>
    public async Task<SearchResultResponse> Handle(SearchAvailableRoomsQuery request, CancellationToken cancellationToken)
    {
        var searchRequest = request.Request;

        logger.LogInformation(
            $"Searching for available rooms in hotel {searchRequest.HotelId} for period {searchRequest.CheckInDate:yyyy-MM-dd} - {searchRequest.CheckOutDate:yyyy-MM-dd}. " +
            $"Requires: {searchRequest.NumberOfRooms} rooms for {searchRequest.NumberOfAdults} adults");

        // Get all rooms and bookings concurrently from their respective repositories
        var roomsTask = roomRepository.GetByHotelIdAsync(searchRequest.HotelId, cancellationToken);
        var bookingsTask = bookingRepository.GetByHotelIdAsync(searchRequest.HotelId, cancellationToken);
        
        await Task.WhenAll(roomsTask, bookingsTask);
        
        var roomsInHotel = roomsTask.Result
            .Where(r => r.NumberOfSubRooms >= searchRequest.NumberOfRooms)
            .ToList();

        logger.LogInformation($"Hotel {searchRequest.HotelId} has {roomsInHotel.Count} rooms in with number of sub rooms {searchRequest.NumberOfRooms}");

        // Get all bookings for this hotel for the specified period
        var bookings = bookingsTask.Result
            .Where(b => b.CheckInDate < searchRequest.CheckOutDate &&
                        b.CheckOutDate > searchRequest.CheckInDate)
            .ToLookup(b => b.RoomId);

        logger.LogInformation($"{bookings.Count} rooms are booked for the specified period");

        // Calculate minimum capacity based on adults and children
        var minimumCapacity = searchRequest.NumberOfAdults + (searchRequest.ChildrenAges?.Count ?? 0);
        
        logger.LogInformation($"Minimum required room capacity: {minimumCapacity}");

        // Filter available rooms by capacity
        var roomsWithCapacity = roomsInHotel
            .Where(r => r.Capacity >= minimumCapacity);

        // Filter out booked rooms and map to response DTOs
        var availableRooms = roomsWithCapacity
            .Where(r => !bookings.Contains(r.Id))
            .Select(r => MapRoomToResponse(r, searchRequest))
            .ToList();

        logger.LogInformation($"Found {availableRooms.Count} available rooms");

        return new SearchResultResponse(availableRooms, availableRooms.Count);
    }

    private static RoomPlanDto MapRoomPlanToDto(RoomPlan roomPlan, Room room, SearchAvailableRoomsRequest request)
    {
        var numberOfNights = (request.CheckOutDate - request.CheckInDate).Days;
        var totalPrice = room.PricePerNight * numberOfNights * roomPlan.PriceFactor;
        var cancellationPolicy = roomPlan.CancellationPolicy;
        DateTime? freeRefundUntil = cancellationPolicy?.FreeRefundUntilDays != null
            ? request.CheckInDate.AddDays(cancellationPolicy.FreeRefundUntilDays.Value)
            : null;

        return new RoomPlanDto(
            roomPlan.Id,
            roomPlan.Name,
            totalPrice,
            new CancellationPolicyDto(
                cancellationPolicy?.Type.ToString() ?? CancellationPolicyType.NoRefund.ToString(),
                freeRefundUntil),
            roomPlan.MealIncluded.ToString());
    }

    private static RoomAvailableResponse MapRoomToResponse(Room room, SearchAvailableRoomsRequest request)
    {
        return new RoomAvailableResponse(
            room.Id,
            room.Number,
            room.RoomPlanLinks
                .Select(p => MapRoomPlanToDto(p.RoomPlan, room, request))
                .ToList());
    }
}
