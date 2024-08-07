using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTaxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Taxes");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Taxes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Taxes");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Taxes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
