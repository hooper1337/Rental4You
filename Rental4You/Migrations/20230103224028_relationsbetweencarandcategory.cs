using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class relationsbetweencarandcategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "type",
                table: "vehicles");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "categories");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_CategoryId",
                table: "vehicles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_categories_CategoryId",
                table: "vehicles",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_categories_CategoryId",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_vehicles_CategoryId",
                table: "vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "vehicles");

            migrationBuilder.RenameTable(
                name: "categories",
                newName: "Category");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "vehicles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_companies_CompanyId",
                table: "vehicles",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id");
        }
    }
}
