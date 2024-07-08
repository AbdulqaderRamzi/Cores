using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyAndPaymentMethodToPurchaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Purchases");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CurrencyId",
                table: "Purchases",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PaymentMethodId",
                table: "Purchases",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Currencies_CurrencyId",
                table: "Purchases",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_PaymentMethods_PaymentMethodId",
                table: "Purchases",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Currencies_CurrencyId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_PaymentMethods_PaymentMethodId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_CurrencyId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_PaymentMethodId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Purchases");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Purchases",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Purchases",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
