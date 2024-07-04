using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddModifiedByColumnToEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Events",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ModifiedById",
                table: "Events",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_ModifiedById",
                table: "Events",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_ModifiedById",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ModifiedById",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Events");
        }
    }
}
