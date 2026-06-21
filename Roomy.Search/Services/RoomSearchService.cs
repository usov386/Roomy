using Microsoft.Extensions.Logging;
using Roomy.Data.Enums;
using Roomy.Data.Models;
using Roomy.Data.Repositories;
using Roomy.Search.DTOs;

namespace Roomy.Search.Services;

/// <summary>
/// Service for searching available rooms in hotels
/// </summary>
/// <remarks>
/// Constructor for the service
/// </remarks>
/// <param name="roomRepository">Repository for room operations</param>
/// <param name="bookingRepository">Repository for booking operations</param>
/// <param name="logger">Logger for logging</param>
public class RoomSearchService(IRoomRepository roomRepository,
    IBookingRepository bookingRepository,
    ILogger<RoomSearchService> logger) : IRoomSearchService
{
    /// <summary>
    /// Searches for available rooms in a hotel based on specified parameters
    /// </summary>
    /// <param name="request">Search parameters</param>
    /// <returns>Search result with available rooms</returns>
    public async Task<SearchResultResponse> SearchAvailableRoomsAsync(SearchAvailableRoomsRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Searching for available rooms in hotel {HotelId} for period {CheckIn:yyyy-MM-dd} - {CheckOut:yyyy-MM-dd}. " +
            "Requires: {NumberOfRooms} rooms for {NumberOfAdults} adults",
            request.HotelId, request.CheckInDate, request.CheckOutDate, 
            request.NumberOfRooms, request.NumberOfAdults);

        // Get all rooms and bookings concurrently from their respective repositories
        var roomsTask = roomRepository.GetByHotelIdAsync(request.HotelId, cancellationToken);
        var bookingsTask = bookingRepository.GetByHotelIdAsync(request.HotelId, cancellationToken);
        
        await Task.WhenAll(roomsTask, bookingsTask);
        
        var roomsInHotel = roomsTask.Result
            .Where(r => r.NumberOfSubRooms >= request.NumberOfRooms)
            .ToList();

        logger.LogInformation($"Hotel {request.HotelId} has {roomsInHotel.Count} rooms in with number of sub rooms {request.NumberOfRooms}");

        // Get all bookings for this hotel for the specified period
        var bookings = bookingsTask.Result
            .Where(b => b.CheckInDate < request.CheckOutDate &&
                        b.CheckOutDate > request.CheckInDate)
            .ToLookup(b => b.RoomId);

        logger.LogInformation("{BookedCount} rooms are booked for the specified period", 
            bookings.Count);

        // Calculate minimum capacity based on adults and children
        var minimumCapacity = request.NumberOfAdults + (request.ChildrenAges?.Count ?? 0);
        
        logger.LogInformation("Minimum required room capacity: {MinCapacity}", minimumCapacity);

        // Filter available rooms by capacity
        var roomsWithCapacity = roomsInHotel
            .Where(r => r.Capacity >= minimumCapacity);

        // Filter out booked rooms and map to response DTOs
        var availableRooms = roomsWithCapacity
            .Where(r => !bookings.Contains(r.Id))
            .Select(r => MapRoomToResponse(r, request))
            .ToList();

        logger.LogInformation("Found {AvailableCount} available rooms", availableRooms.Count);

        return new SearchResultResponse(availableRooms, availableRooms.Count);
    }

    private static RoomPlanDto MapRoomPlanToDto(RoomPlanLink roomPlanLink, SearchAvailableRoomsRequest request)
    {
        var numberOfNights = (request.CheckOutDate - request.CheckInDate).Days;
        var totalPrice = roomPlanLink.RoomPlan.PricePerNight * numberOfNights;
        var cancellationPolicy = roomPlanLink.RoomPlan.CancellationPolicy;
        DateTime? freeRefundUntil = cancellationPolicy?.FreeRefundUntilDays != null
            ? request.CheckInDate.AddDays(cancellationPolicy.FreeRefundUntilDays.Value)
            : null;

        return new RoomPlanDto(
            roomPlanLink.RoomPlan.Id,
            roomPlanLink.RoomPlan.Name,
            totalPrice,
            new CancellationPolicyDto(
                cancellationPolicy?.Type.ToString() ?? CancellationPolicyType.NoRefund.ToString(),
                freeRefundUntil),
            roomPlanLink.RoomPlan.MealIncluded.ToString());
    }

    private static RoomAvailableResponse MapRoomToResponse(Room room, SearchAvailableRoomsRequest request)
    {
        return new RoomAvailableResponse(
            room.Number,
            room.RoomPlanLinks
                .Select(p => MapRoomPlanToDto(p, request))
                .ToList());
    }
}
