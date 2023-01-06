using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class ihatemicrosoft : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "vehicleStateId",
                table: "vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_vehicleStateId",
                table: "vehicles",
                column: "vehicleStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicleStates_vehicleStateId",
                table: "vehicles",
                column: "vehicleStateId",
                principalTable: "vehicleStates",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicleStates_vehicleStateId",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_vehicles_vehicleStateId",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "vehicleStateId",
                table: "vehicles");
        }
    }
}
