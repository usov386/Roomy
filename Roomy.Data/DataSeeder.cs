using Microsoft.EntityFrameworkCore;
using Roomy.Data.Enums;
using Roomy.Data.Models;

namespace Roomy.Data;

/// <summary>
/// Class for populating the database with test data
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Populates the database with test data if it's empty
    /// </summary>
    

    /// <summary>
    /// Populates the database with test data if it's empty
    /// </summary>
    /// <param name="context">AppDbContext</param>
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if the database already contains hotels
        if (context.Hotels.Any())
        {
            return; // Database is already populated
        }

        // Create users
        var users = new List<User>
        {
            new User { Name = "Іван Петренко", Email = "ivan@example.com" },
            new User { Name = "Марія Сидоренко", Email = "maria@example.com" },
            new User { Name = "Петро Коваленко", Email = "petro@example.com" },
            new User { Name = "Ольга Шевченко", Email = "olga@example.com" },
            new User { Name = "Андрій Бондаренко", Email = "andrii@example.com" }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Create hotels with fixed IDs for testing
        var hotelId1 = new Guid("10000000-0000-0000-0000-000000000001");
        var hotelId2 = new Guid("20000000-0000-0000-0000-000000000002");
        var hotelId3 = new Guid("30000000-0000-0000-0000-000000000003");

        var hotels = new List<Hotel>
        {
            new Hotel 
            { 
                Id = hotelId1, 
                Name = "Grand Hotel Kyiv", 
                City = "Київ", 
                Address = "вул. Хрещатик, 2" 
            },
            new Hotel 
            { 
                Id = hotelId2, 
                Name = "Budget Inn Lviv", 
                City = "Львів", 
                Address = "вул. Франка, 5" 
            },
            new Hotel 
            { 
                Id = hotelId3, 
                Name = "Luxury Resort Odesa", 
                City = "Одеса", 
                Address = "Морський бульвар, 10" 
            }
        };

        await context.Hotels.AddRangeAsync(hotels);
        await context.SaveChangesAsync();

        // Create rooms for each hotel
        var rooms = new List<Room>();

        // Rooms for Grand Hotel Kyiv
        var grandHotel = hotels[0];
        rooms.AddRange(new[]
        {
            new Room 
            { 
                HotelId = grandHotel.Id, 
                Number = "101", 
                Type = RoomType.Single, 
                Capacity = 1,
                NumberOfSubRooms = 1
            },
            new Room 
            { 
                HotelId = grandHotel.Id, 
                Number = "102", 
                Type = RoomType.Double, 
                Capacity = 2,
                NumberOfSubRooms = 1
            },
            new Room 
            { 
                HotelId = grandHotel.Id, 
                Number = "103", 
                Type = RoomType.Suite, 
                Capacity = 3,
                NumberOfSubRooms = 2
            },
            new Room 
            { 
                HotelId = grandHotel.Id, 
                Number = "104", 
                Type = RoomType.Family, 
                Capacity = 4,
                NumberOfSubRooms = 3
            },
            new Room 
            { 
                HotelId = grandHotel.Id, 
                Number = "105", 
                Type = RoomType.Double, 
                Capacity = 2,
                NumberOfSubRooms = 1
            }
        });

        // Rooms for Budget Inn Lviv
        var budgetHotel = hotels[1];
        rooms.AddRange(new[]
        {
            new Room 
            { 
                HotelId = budgetHotel.Id, 
                Number = "201", 
                Type = RoomType.Single, 
                Capacity = 1,
                NumberOfSubRooms = 1
            },
            new Room 
            { 
                HotelId = budgetHotel.Id, 
                Number = "202", 
                Type = RoomType.Double, 
                Capacity = 2,
                NumberOfSubRooms = 1
            },
            new Room 
            { 
                HotelId = budgetHotel.Id, 
                Number = "203", 
                Type = RoomType.Double, 
                Capacity = 2,
                NumberOfSubRooms = 1
            },
            new Room 
            { 
                HotelId = budgetHotel.Id, 
                Number = "204", 
                Type = RoomType.Family, 
                Capacity = 3,
                NumberOfSubRooms = 2
            }
        });

        // Rooms for Luxury Resort Odesa
        var luxuryHotel = hotels[2];
        rooms.AddRange(new[]
        {
            new Room 
            { 
                HotelId = luxuryHotel.Id, 
                Number = "301", 
                Type = RoomType.Suite, 
                Capacity = 2,
                NumberOfSubRooms = 2
            },
            new Room 
            { 
                HotelId = luxuryHotel.Id, 
                Number = "302", 
                Type = RoomType.Suite, 
                Capacity = 3,
                NumberOfSubRooms = 2
            },
            new Room 
            { 
                HotelId = luxuryHotel.Id, 
                Number = "303", 
                Type = RoomType.Family, 
                Capacity = 4,
                NumberOfSubRooms = 3
            },
            new Room 
            { 
                HotelId = luxuryHotel.Id, 
                Number = "304", 
                Type = RoomType.Double, 
                Capacity = 2,
                NumberOfSubRooms = 1
            }
        });

        await context.Rooms.AddRangeAsync(rooms);
        await context.SaveChangesAsync();

        // Create global pricing plans (not tied to specific rooms)
        var standardPlan = new RoomPlan
        {
            Name = "Standard",
            PricePerNight = 100m, // Fixed standard price
            MealIncluded = MealIncluded.Breakfast,
            CreatedAt = DateTime.UtcNow
        };

        // Standard cancellation policy - free until 7 days before
        var standardPolicy = new CancellationPolicy
        {
            Type = CancellationPolicyType.FreeUntilDateTime,
            FreeRefundUntilDays = 7
        };
        standardPlan.CancellationPolicy = standardPolicy;

        await context.RoomPlans.AddAsync(standardPlan);
        await context.SaveChangesAsync();

        var budgetPlan = new RoomPlan
        {
            Name = "Budget (Non-Refundable)",
            PricePerNight = 85m, // 15% discount
            MealIncluded = MealIncluded.None,
            CreatedAt = DateTime.UtcNow
        };

        // Budget cancellation policy - no refunds
        var budgetPolicy = new CancellationPolicy
        {
            Type = CancellationPolicyType.NoRefund            
        };
        budgetPlan.CancellationPolicy = budgetPolicy;

        await context.RoomPlans.AddAsync(budgetPlan);
        await context.SaveChangesAsync();

        var premiumPlan = new RoomPlan
        {
            Name = "Premium",
            PricePerNight = 115m, // 15% premium
            MealIncluded = MealIncluded.BreakfastLunchDinner,
            CreatedAt = DateTime.UtcNow
        };

        // Premium cancellation policy - free cancellation until day before check-in
        var premiumPolicy = new CancellationPolicy
        {
            Type = CancellationPolicyType.FreeUntilDateTime,
            FreeRefundUntilDays = 1
        };
        premiumPlan.CancellationPolicy = premiumPolicy;

        await context.RoomPlans.AddAsync(premiumPlan);
        await context.SaveChangesAsync();

        // Link rooms to plans via junction table
        var roomRoomPlans = new List<RoomPlanLink>();
        foreach (var room in rooms)
        {
            roomRoomPlans.Add(new RoomPlanLink { RoomId = room.Id, RoomPlanId = standardPlan.Id });
            roomRoomPlans.Add(new RoomPlanLink { RoomId = room.Id, RoomPlanId = budgetPlan.Id });
            roomRoomPlans.Add(new RoomPlanLink { RoomId = room.Id, RoomPlanId = premiumPlan.Id });
        }

        await context.RoomPlanLinks.AddRangeAsync(roomRoomPlans);
        await context.SaveChangesAsync();

        // Bookings
        var today = DateTime.Today;
        var bookings = new List<Booking>
        {
            // Active bookings
            new Booking 
            { 
                HotelId = hotelId1,
                RoomId = rooms[0].Id, 
                UserId = users[0].Id, 
                CheckInDate = today.AddDays(1), 
                CheckOutDate = today.AddDays(3)
            },
            new Booking 
            { 
                HotelId = hotelId1,
                RoomId = rooms[1].Id, 
                UserId = users[1].Id, 
                CheckInDate = today.AddDays(5), 
                CheckOutDate = today.AddDays(8) 
            },
            new Booking 
            { 
                HotelId = hotelId1,
                RoomId = rooms[2].Id, 
                UserId = users[2].Id, 
                CheckInDate = today.AddDays(10), 
                CheckOutDate = today.AddDays(15) 
            },
            // Минулі бронювання
            new Booking 
            { 
                HotelId = hotelId1,
                RoomId = rooms[3].Id, 
                UserId = users[3].Id, 
                CheckInDate = today.AddDays(-10), 
                CheckOutDate = today.AddDays(-5) 
            },
            new Booking 
            { 
                HotelId = hotelId1,
                RoomId = rooms[4].Id, 
                UserId = users[4].Id, 
                CheckInDate = today.AddDays(-3), 
                CheckOutDate = today.AddDays(-1) 
            },
            // Більше бронювань для різних номерів
            new Booking 
            { 
                HotelId = hotelId2,
                RoomId = rooms[5].Id, 
                UserId = users[0].Id, 
                CheckInDate = today.AddDays(2), 
                CheckOutDate = today.AddDays(6) 
            },
            new Booking 
            { 
                HotelId = hotelId2,
                RoomId = rooms[6].Id, 
                UserId = users[1].Id, 
                CheckInDate = today.AddDays(7), 
                CheckOutDate = today.AddDays(10) 
            },
            new Booking 
            { 
                HotelId = hotelId2,
                RoomId = rooms[8].Id, 
                UserId = users[2].Id, 
                CheckInDate = today.AddDays(12), 
                CheckOutDate = today.AddDays(14) 
            }
        };

        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
    }
}
