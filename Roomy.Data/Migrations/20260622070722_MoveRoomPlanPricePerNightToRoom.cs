using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomy.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveRoomPlanPricePerNightToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "RoomPlans");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceFactor",
                table: "RoomPlans",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "PriceFactor",
                table: "RoomPlans");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                table: "RoomPlans",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
