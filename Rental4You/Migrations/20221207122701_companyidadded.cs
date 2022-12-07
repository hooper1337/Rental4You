using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class companyidadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_companies_companyId",
                table: "vehicles");

            migrationBuilder.RenameColumn(
                name: "companyId",
                table: "vehicles",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_vehicles_companyId",
                table: "vehicles",
                newName: "IX_vehicles_CompanyId");

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

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "vehicles",
                newName: "companyId");

            migrationBuilder.RenameIndex(
                name: "IX_vehicles_CompanyId",
                table: "vehicles",
                newName: "IX_vehicles_companyId");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_companies_companyId",
                table: "vehicles",
                column: "companyId",
                principalTable: "companies",
                principalColumn: "Id");
        }
    }
}
