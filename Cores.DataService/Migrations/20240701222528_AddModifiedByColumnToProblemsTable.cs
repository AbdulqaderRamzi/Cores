using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddModifiedByColumnToProblemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Problems",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ModifiedById",
                table: "Problems",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_AspNetUsers_ModifiedById",
                table: "Problems",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_AspNetUsers_ModifiedById",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ModifiedById",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Problems");
        }
    }
}
