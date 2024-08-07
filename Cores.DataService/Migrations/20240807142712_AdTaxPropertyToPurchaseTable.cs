using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AdTaxPropertyToPurchaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaxId",
                table: "Purchases",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TaxId",
                table: "Purchases",
                column: "TaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Taxes_TaxId",
                table: "Purchases",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Taxes_TaxId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TaxId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Purchases");
        }
    }
}
