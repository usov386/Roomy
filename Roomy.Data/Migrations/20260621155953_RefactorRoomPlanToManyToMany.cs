using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomy.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRoomPlanToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomPlans_Rooms_RoomId",
                table: "RoomPlans");

            migrationBuilder.DropIndex(
                name: "IX_RoomPlans_RoomId_Name",
                table: "RoomPlans");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "RoomPlans");

            migrationBuilder.CreateTable(
                name: "RoomPlanLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPlanLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomPlanLinks_RoomPlans_RoomPlanId",
                        column: x => x.RoomPlanId,
                        principalTable: "RoomPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomPlanLinks_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomPlans_Name",
                table: "RoomPlans",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomPlanLinks_RoomId_RoomPlanId",
                table: "RoomPlanLinks",
                columns: new[] { "RoomId", "RoomPlanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomPlanLinks_RoomPlanId",
                table: "RoomPlanLinks",
                column: "RoomPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomPlanLinks");

            migrationBuilder.DropIndex(
                name: "IX_RoomPlans_Name",
                table: "RoomPlans");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "RoomPlans",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RoomPlans_RoomId_Name",
                table: "RoomPlans",
                columns: new[] { "RoomId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomPlans_Rooms_RoomId",
                table: "RoomPlans",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
