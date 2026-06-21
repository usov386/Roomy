using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HotelId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            // Update existing bookings with HotelId from their associated rooms
            migrationBuilder.Sql(@"
                UPDATE Bookings
                SET HotelId = r.HotelId
                FROM Bookings b
                INNER JOIN Rooms r ON b.RoomId = r.Id
            ");

            // Make the column non-nullable after data is populated
            migrationBuilder.AlterColumn<Guid>(
                name: "HotelId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Bookings");
        }
    }
}
