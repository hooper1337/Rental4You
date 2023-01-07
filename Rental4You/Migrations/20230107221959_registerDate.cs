using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class registerDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "available",
                table: "managers");

            migrationBuilder.DropColumn(
                name: "available",
                table: "employees");

            migrationBuilder.AddColumn<DateTime>(
                name: "registerDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "registerDate",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "available",
                table: "managers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "available",
                table: "employees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
