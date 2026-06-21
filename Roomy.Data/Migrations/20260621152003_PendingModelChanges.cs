using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomy.Data.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RoomPlans");

            migrationBuilder.DropColumn(
                name: "FreeRefundUntilDateTime",
                table: "CancellationPolicies");

            migrationBuilder.AddColumn<int>(
                name: "FreeRefundUntilDays",
                table: "CancellationPolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeRefundUntilDays",
                table: "CancellationPolicies");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RoomPlans",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FreeRefundUntilDateTime",
                table: "CancellationPolicies",
                type: "datetime2",
                nullable: true);
        }
    }
}
