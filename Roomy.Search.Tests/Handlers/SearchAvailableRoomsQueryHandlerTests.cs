using Moq;
using Xunit;
using FluentAssertions;
using Roomy.Search.Handlers;
using Roomy.Search.Queries;
using Roomy.Search.DTOs;
using Roomy.Data.Models;
using Roomy.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Roomy.Search.Tests.Handlers;

public class SearchAvailableRoomsQueryHandlerTests
{
    private readonly Mock<IRoomRepository> _mockRoomRepository;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<ILogger<SearchAvailableRoomsQueryHandler>> _mockLogger;
    private readonly SearchAvailableRoomsQueryHandler _handler;
    private readonly Guid _hotelId = Guid.NewGuid();

    public SearchAvailableRoomsQueryHandlerTests()
    {
        _mockRoomRepository = new Mock<IRoomRepository>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<SearchAvailableRoomsQueryHandler>>();

        _handler = new SearchAvailableRoomsQueryHandler(
            _mockRoomRepository.Object,
            _mockBookingRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithNoBookings_ReturnsAllAvailableRooms()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var rooms = new List<Room>
        {
            CreateRoom(capacity: 4, numberOfSubRooms: 1),
            CreateRoom(capacity: 2, numberOfSubRooms: 1),
            CreateRoom(capacity: 6, numberOfSubRooms: 2)
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AvailableRooms.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ExcludesRoomsWithInsufficientCapacity()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var numberOfAdults = 2;
        var childrenAges = new List<int> { 5, 8 };
        var minimumCapacity = numberOfAdults + childrenAges.Count; // 4

        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: numberOfAdults,
            ChildrenAges: childrenAges);

        var rooms = new List<Room>
        {
            CreateRoom(capacity: 2, numberOfSubRooms: 1), // Insufficient
            CreateRoom(capacity: 3, numberOfSubRooms: 1), // Insufficient
            CreateRoom(capacity: 4, numberOfSubRooms: 1), // Sufficient
            CreateRoom(capacity: 6, numberOfSubRooms: 1)  // Sufficient
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ExcludesRoomsWithInsufficientSubRooms()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 2, // Requires 2 sub-rooms
            NumberOfAdults: 4,
            ChildrenAges: null);

        var rooms = new List<Room>
        {
            CreateRoom(capacity: 4, numberOfSubRooms: 1), // Insufficient
            CreateRoom(capacity: 8, numberOfSubRooms: 2), // Sufficient
            CreateRoom(capacity: 6, numberOfSubRooms: 3)  // Sufficient
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ExcludesBookedRoomsForTheSearchPeriod()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(5);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var room1 = CreateRoom(capacity: 4, numberOfSubRooms: 1);
        var room2 = CreateRoom(capacity: 4, numberOfSubRooms: 1);
        var room3 = CreateRoom(capacity: 4, numberOfSubRooms: 1);

        var rooms = new List<Room> { room1, room2, room3 };

        var bookings = new List<Booking>
        {
            CreateBooking(room1.Id, _hotelId, checkInDate.AddDays(1), checkOutDate.AddDays(-1)) // Overlaps
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_IncludesRoomsWithNonOverlappingBookings()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(10);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var room1 = CreateRoom(capacity: 4, numberOfSubRooms: 1);

        var bookings = new List<Booking>
        {
            // Booking before search period
            CreateBooking(room1.Id, _hotelId, checkInDate.AddDays(-5), checkInDate.AddDays(-1)),
            // Booking after search period
            CreateBooking(room1.Id, _hotelId, checkOutDate.AddDays(1), checkOutDate.AddDays(5))
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Room> { room1 });
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithAllRoomsBooked_ReturnsEmptyList()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(5);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var room1 = CreateRoom(capacity: 4, numberOfSubRooms: 1);
        var room2 = CreateRoom(capacity: 4, numberOfSubRooms: 1);

        var rooms = new List<Room> { room1, room2 };

        var bookings = new List<Booking>
        {
            CreateBooking(room1.Id, _hotelId, checkInDate, checkOutDate),
            CreateBooking(room2.Id, _hotelId, checkInDate, checkOutDate)
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithNoRoomsInHotel_ReturnsEmptyList()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Room>());
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CalculatesMinimumCapacityCorrectly()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var numberOfAdults = 1;
        var childrenAges = new List<int> { 3, 7, 10 };
        var expectedMinimumCapacity = numberOfAdults + childrenAges.Count; // 4

        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: numberOfAdults,
            ChildrenAges: childrenAges);

        var rooms = new List<Room>
        {
            CreateRoom(capacity: 3, numberOfSubRooms: 1),  // Below minimum
            CreateRoom(capacity: 4, numberOfSubRooms: 1),  // At minimum
            CreateRoom(capacity: 5, numberOfSubRooms: 1)   // Above minimum
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithPartialBookingOverlap_ExcludesRoom()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(10);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var room1 = CreateRoom(capacity: 4, numberOfSubRooms: 1);

        // Booking that overlaps only at the start
        var bookings = new List<Booking>
        {
            CreateBooking(room1.Id, _hotelId, checkInDate.AddDays(-1), checkInDate.AddDays(1))
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Room> { room1 });
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ReturnsRoomWithPlans()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var room = CreateRoomWithPlans(capacity: 4, numberOfSubRooms: 1, numberOfPlans: 2);
        var rooms = new List<Room> { room };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvailableRooms.Should().HaveCount(1);
        result.AvailableRooms[0].AvailablePlans.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ComplexScenarioWithMixedRooms()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(5);
        var checkOutDate = checkInDate.AddDays(3);
        var numberOfAdults = 2;
        var childrenAges = new List<int> { 6 };
        var minimumCapacity = numberOfAdults + childrenAges.Count; // 3

        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: numberOfAdults,
            ChildrenAges: childrenAges);

        // Create various rooms
        var room1 = CreateRoom(capacity: 2, numberOfSubRooms: 1);          // Insufficient capacity
        var room2 = CreateRoom(capacity: 3, numberOfSubRooms: 1);          // Sufficient capacity
        var room3 = CreateRoom(capacity: 4, numberOfSubRooms: 1);          // Sufficient capacity
        var room4 = CreateRoom(capacity: 5, numberOfSubRooms: 1);          // Sufficient capacity
        var room5 = CreateRoom(capacity: 6, numberOfSubRooms: 1);          // Sufficient capacity

        var rooms = new List<Room> { room1, room2, room3, room4, room5 };

        // Some rooms are booked
        var bookings = new List<Booking>
        {
            CreateBooking(room2.Id, _hotelId, checkInDate, checkOutDate),  // Booked
            CreateBooking(room4.Id, _hotelId, checkInDate.AddDays(-5), checkInDate.AddDays(-1)) // Not overlapping
        };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        // room1: Excluded (insufficient capacity)
        // room2: Excluded (booked)
        // room3: Available
        // room4: Available (booking is not overlapping)
        // room5: Available
        result.AvailableRooms.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_LogsSearchInformation()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var rooms = new List<Room> { CreateRoom(capacity: 4, numberOfSubRooms: 1) };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ConcurrentlyFetchesRoomsAndBookings()
    {
        // Arrange
        var checkInDate = DateTime.Today.AddDays(1);
        var checkOutDate = checkInDate.AddDays(3);
        var request = new SearchAvailableRoomsRequest(
            _hotelId,
            checkInDate,
            checkOutDate,
            NumberOfRooms: 1,
            NumberOfAdults: 2,
            ChildrenAges: null);

        var rooms = new List<Room> { CreateRoom(capacity: 4, numberOfSubRooms: 1) };

        _mockRoomRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rooms);
        _mockBookingRepository.Setup(b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        var query = new SearchAvailableRoomsQuery { Request = request };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRoomRepository.Verify(
            r => r.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockBookingRepository.Verify(
            b => b.GetByHotelIdAsync(_hotelId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // Helper methods
    private Room CreateRoom(int capacity, int numberOfSubRooms)
    {
        return new Room
        {
            Id = Guid.NewGuid(),
            HotelId = _hotelId,
            Number = $"Room-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Type = Roomy.Data.Enums.RoomType.Single,
            Capacity = capacity,
            NumberOfSubRooms = numberOfSubRooms,
            Bookings = new List<Booking>(),
            RoomPlanLinks = new List<RoomPlanLink>()
        };
    }

    private Room CreateRoomWithPlans(int capacity, int numberOfSubRooms, int numberOfPlans)
    {
        var room = CreateRoom(capacity, numberOfSubRooms);
        var plans = Enumerable.Range(1, numberOfPlans)
            .Select(i => CreateRoomPlanLink(room.Id))
            .ToList();
        room.RoomPlanLinks = plans;
        return room;
    }

    private Booking CreateBooking(Guid roomId, Guid hotelId, DateTime checkIn, DateTime checkOut)
    {
        return new Booking
        {
            Id = new Random().Next(1, 10000),
            RoomId = roomId,
            HotelId = hotelId,
            UserId = 1,
            CheckInDate = checkIn,
            CheckOutDate = checkOut
        };
    }

    private RoomPlanLink CreateRoomPlanLink(Guid roomId)
    {
        return new RoomPlanLink
        {
            RoomId = roomId,
            RoomPlanId = new Random().Next(1, 100),
            RoomPlan = new RoomPlan
            {
                Id = new Random().Next(1, 100),
                Name = $"Plan-{Guid.NewGuid().ToString().Substring(0, 8)}",
                PricePerNight = 100m,
                MealIncluded = Roomy.Data.Enums.MealIncluded.None,
                CancellationPolicy = new CancellationPolicy
                {
                    Id = 1,
                    Type = Roomy.Data.Models.CancellationPolicyType.NoRefund,
                    FreeRefundUntilDays = null
                }
            }
        };
    }
}
