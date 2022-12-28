using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class employescheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_applicationUserId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_companies_companyId",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "employees");

            migrationBuilder.RenameColumn(
                name: "companyId",
                table: "employees",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_companyId",
                table: "employees",
                newName: "IX_employees_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_applicationUserId",
                table: "employees",
                newName: "IX_employees_applicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "applicationUserId",
                table: "employees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "available",
                table: "employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_employees",
                table: "employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_AspNetUsers_applicationUserId",
                table: "employees",
                column: "applicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_companies_CompanyId",
                table: "employees",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_AspNetUsers_applicationUserId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_companies_CompanyId",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employees",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "available",
                table: "employees");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employee");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Employee",
                newName: "companyId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_CompanyId",
                table: "Employee",
                newName: "IX_Employee_companyId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_applicationUserId",
                table: "Employee",
                newName: "IX_Employee_applicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "applicationUserId",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_applicationUserId",
                table: "Employee",
                column: "applicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_companies_companyId",
                table: "Employee",
                column: "companyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
