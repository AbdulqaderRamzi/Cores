using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddStartAndEndDateInHoliday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Holidays",
                newName: "StartAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndAt",
                table: "Holidays",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndAt",
                table: "Holidays");

            migrationBuilder.RenameColumn(
                name: "StartAt",
                table: "Holidays",
                newName: "Date");
        }
    }
}
