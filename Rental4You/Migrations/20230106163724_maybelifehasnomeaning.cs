using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class maybelifehasnomeaning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservations_VehicleState_vehicleStateDeliveryId",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_VehicleState_vehicleStateRetrievalId",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleState_AspNetUsers_ApplicationUserID",
                table: "VehicleState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleState",
                table: "VehicleState");

            migrationBuilder.RenameTable(
                name: "VehicleState",
                newName: "vehicleStates");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleState_ApplicationUserID",
                table: "vehicleStates",
                newName: "IX_vehicleStates_ApplicationUserID");

            migrationBuilder.AlterColumn<bool>(
                name: "Damage",
                table: "vehicleStates",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_vehicleStates",
                table: "vehicleStates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_vehicleStates_vehicleStateDeliveryId",
                table: "reservations",
                column: "vehicleStateDeliveryId",
                principalTable: "vehicleStates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_vehicleStates_vehicleStateRetrievalId",
                table: "reservations",
                column: "vehicleStateRetrievalId",
                principalTable: "vehicleStates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicleStates_AspNetUsers_ApplicationUserID",
                table: "vehicleStates",
                column: "ApplicationUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservations_vehicleStates_vehicleStateDeliveryId",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_vehicleStates_vehicleStateRetrievalId",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicleStates_AspNetUsers_ApplicationUserID",
                table: "vehicleStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vehicleStates",
                table: "vehicleStates");

            migrationBuilder.RenameTable(
                name: "vehicleStates",
                newName: "VehicleState");

            migrationBuilder.RenameIndex(
                name: "IX_vehicleStates_ApplicationUserID",
                table: "VehicleState",
                newName: "IX_VehicleState_ApplicationUserID");

            migrationBuilder.AlterColumn<bool>(
                name: "Damage",
                table: "VehicleState",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleState",
                table: "VehicleState",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_VehicleState_vehicleStateDeliveryId",
                table: "reservations",
                column: "vehicleStateDeliveryId",
                principalTable: "VehicleState",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_VehicleState_vehicleStateRetrievalId",
                table: "reservations",
                column: "vehicleStateRetrievalId",
                principalTable: "VehicleState",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleState_AspNetUsers_ApplicationUserID",
                table: "VehicleState",
                column: "ApplicationUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
