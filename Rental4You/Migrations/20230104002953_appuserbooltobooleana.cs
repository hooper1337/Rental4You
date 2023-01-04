using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class appuserbooltobooleana : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "vehicles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "available",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "available",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
