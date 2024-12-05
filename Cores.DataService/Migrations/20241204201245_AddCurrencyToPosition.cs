using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryRange",
                table: "Positions");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageSalary",
                table: "Positions",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Positions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CurrencyId",
                table: "Positions",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Departments_CurrencyId",
                table: "Positions",
                column: "CurrencyId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Departments_CurrencyId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_CurrencyId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "AverageSalary",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Positions");

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryRange",
                table: "Positions",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
