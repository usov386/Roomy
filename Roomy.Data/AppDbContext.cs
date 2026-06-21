using Microsoft.EntityFrameworkCore;
using Roomy.Data.Models;

namespace Roomy.Data;

/// <summary>
/// Entity Framework Core context for the hotel booking system
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Constructor for the context
    /// </summary>
    /// <param name="options">Options for configuring DbContext</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet for users
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// DbSet for hotels
    /// </summary>
    public DbSet<Hotel> Hotels { get; set; }

    /// <summary>
    /// DbSet for rooms
    /// </summary>
    public DbSet<Room> Rooms { get; set; }

    /// <summary>
    /// DbSet for bookings
    /// </summary>
    public DbSet<Booking> Bookings { get; set; }

    /// <summary>
    /// Configuration of models and their relationships
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Hotel configuration
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(255);
            entity.Property(h => h.City).IsRequired().HasMaxLength(255);
            entity.Property(h => h.Address).IsRequired().HasMaxLength(500);
            entity.HasIndex(h => h.Name).IsUnique();
            
            entity.HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Room configuration
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Number).IsRequired().HasMaxLength(50);
            entity.Property(r => r.Type).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Capacity).IsRequired();
            entity.Property(r => r.PricePerNight).IsRequired().HasPrecision(10, 2);
            entity.Property(r => r.HotelId).IsRequired();
            
            // Unique index: room number is unique within a hotel
            entity.HasIndex(r => new { r.HotelId, r.Number }).IsUnique();
            
            entity.HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.CheckInDate).IsRequired();
            entity.Property(b => b.CheckOutDate).IsRequired();
            entity.Property(b => b.RoomId).IsRequired();
            entity.Property(b => b.UserId).IsRequired();

            // CheckOut must be after CheckIn
            entity.HasCheckConstraint("CK_CheckOutAfterCheckIn", "\"CheckOutDate\" > \"CheckInDate\"");

            entity.HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for fast lookup of bookings by dates
            entity.HasIndex(b => new { b.RoomId, b.CheckInDate, b.CheckOutDate });
        });
    }
}
