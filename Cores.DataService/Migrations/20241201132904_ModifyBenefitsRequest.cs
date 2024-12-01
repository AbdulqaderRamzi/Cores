using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class ModifyBenefitsRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DependentName",
                table: "BenefitsRequests");

            migrationBuilder.DropColumn(
                name: "Relationship",
                table: "BenefitsRequests");

            migrationBuilder.DropColumn(
                name: "RequestedAmount",
                table: "BenefitsRequests");

            migrationBuilder.DropColumn(
                name: "TravelDetails",
                table: "BenefitsRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DependentName",
                table: "BenefitsRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Relationship",
                table: "BenefitsRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedAmount",
                table: "BenefitsRequests",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TravelDetails",
                table: "BenefitsRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
