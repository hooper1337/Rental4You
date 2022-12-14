using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class RemovedWithdrawAndDeliver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deliver",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "withdraw",
                table: "vehicles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deliver",
                table: "vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "withdraw",
                table: "vehicles",
                type: "datetime2",
                nullable: true);
        }
    }
}
