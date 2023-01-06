using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class goddamnhard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamageDelivery",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "DamageRetrieval",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "EmployerDelivery",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "EmployerRetrieval",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "ObservationsDelivery",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "ObservationsRetrieval",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "RetrievalDate",
                table: "reservations");

            migrationBuilder.RenameColumn(
                name: "NumberOfKmOfVehicleRetrieval",
                table: "reservations",
                newName: "vehicleStateRetrievalId");

            migrationBuilder.RenameColumn(
                name: "NumberOfKmOfVehicleDelivery",
                table: "reservations",
                newName: "vehicleStateDeliveryId");

            migrationBuilder.CreateTable(
                name: "VehicleState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfKmOfVehicle = table.Column<int>(type: "int", nullable: true),
                    Damage = table.Column<bool>(type: "bit", nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleState_AspNetUsers_ApplicationUserID",
                        column: x => x.ApplicationUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservations_vehicleStateDeliveryId",
                table: "reservations",
                column: "vehicleStateDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_vehicleStateRetrievalId",
                table: "reservations",
                column: "vehicleStateRetrievalId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleState_ApplicationUserID",
                table: "VehicleState",
                column: "ApplicationUserID");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservations_VehicleState_vehicleStateDeliveryId",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_VehicleState_vehicleStateRetrievalId",
                table: "reservations");

            migrationBuilder.DropTable(
                name: "VehicleState");

            migrationBuilder.DropIndex(
                name: "IX_reservations_vehicleStateDeliveryId",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "IX_reservations_vehicleStateRetrievalId",
                table: "reservations");

            migrationBuilder.RenameColumn(
                name: "vehicleStateRetrievalId",
                table: "reservations",
                newName: "NumberOfKmOfVehicleRetrieval");

            migrationBuilder.RenameColumn(
                name: "vehicleStateDeliveryId",
                table: "reservations",
                newName: "NumberOfKmOfVehicleDelivery");

            migrationBuilder.AddColumn<string>(
                name: "DamageDelivery",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DamageRetrieval",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployerDelivery",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployerRetrieval",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsDelivery",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsRetrieval",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RetrievalDate",
                table: "reservations",
                type: "datetime2",
                nullable: true);
        }
    }
}
