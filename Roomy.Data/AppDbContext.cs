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
    /// DbSet for room plans
    /// </summary>
    public DbSet<RoomPlan> RoomPlans { get; set; }

    /// <summary>
    /// DbSet for cancellation policies
    /// </summary>
    public DbSet<CancellationPolicy> CancellationPolicies { get; set; }

    /// <summary>
    /// DbSet for room-room plan junction table
    /// </summary>
    public DbSet<RoomPlanLink> RoomPlanLinks { get; set; }

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
            entity.Property(p => p.PricePerNight).IsRequired().HasPrecision(18, 2);
            entity.Property(r => r.Type).IsRequired().HasConversion<int>(); // Store enum as int
            entity.Property(r => r.Capacity).IsRequired();
            entity.Property(r => r.HotelId).IsRequired();
            
            // Unique index: room number is unique within a hotel
            entity.HasIndex(r => new { r.HotelId, r.Number }).IsUnique();
            
            entity.HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.RoomPlanLinks)
                .WithOne(rrp => rrp.Room)
                .HasForeignKey(rrp => rrp.RoomId)
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
            entity.ToTable(t => t.HasCheckConstraint("CK_CheckOutAfterCheckIn", "\"CheckOutDate\" > \"CheckInDate\""));

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

        // RoomPlan configuration
        modelBuilder.Entity<RoomPlan>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
            entity.Property(p => p.PriceFactor).IsRequired().HasPrecision(5, 2);
            entity.Property(p => p.MealIncluded).IsRequired().HasConversion<int>(); // Store enum as int
            entity.Property(p => p.CreatedAt).IsRequired();

            // Unique index: plan name is globally unique
            entity.HasIndex(p => p.Name).IsUnique();

            entity.HasMany(p => p.RoomPlanLinks)
                .WithOne(rrp => rrp.RoomPlan)
                .HasForeignKey(rrp => rrp.RoomPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.CancellationPolicy)
                .WithOne(cp => cp.RoomPlan)
                .HasForeignKey<CancellationPolicy>(cp => cp.RoomPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RoomRoomPlan configuration (junction table)
        modelBuilder.Entity<RoomPlanLink>(entity =>
        {
            entity.HasKey(rrp => rrp.Id);
            entity.Property(rrp => rrp.RoomId).IsRequired();
            entity.Property(rrp => rrp.RoomPlanId).IsRequired();

            // Unique index: each room-plan combination should be unique
            entity.HasIndex(rrp => new { rrp.RoomId, rrp.RoomPlanId }).IsUnique();
        });

        // CancellationPolicy configuration
        modelBuilder.Entity<CancellationPolicy>(entity =>
        {
            entity.HasKey(cp => cp.Id);
            entity.Property(cp => cp.Type).IsRequired().HasConversion<int>(); // Store enum as int
            entity.Property(cp => cp.RoomPlanId).IsRequired();
        });
    }
}
