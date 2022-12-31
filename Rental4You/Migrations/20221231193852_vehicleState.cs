using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class vehicleState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DamageDelivery",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DamageRetrieval",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployerDelivery",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployerRetrieval",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfKmOfVehicleDelivery",
                table: "reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfKmOfVehicleRetrieval",
                table: "reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsDelivery",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ObservationsRetrieval",
                table: "reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamageDelivery",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "DamageRetrieval",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "EmployerDelivery",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "EmployerRetrieval",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "NumberOfKmOfVehicleDelivery",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "NumberOfKmOfVehicleRetrieval",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "ObservationsDelivery",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "ObservationsRetrieval",
                table: "reservations");
        }
    }
}
