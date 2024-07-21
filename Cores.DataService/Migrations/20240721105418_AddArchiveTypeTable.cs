using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddArchiveTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Archives_ArchiveType_ArchiveTypeId",
                table: "Archives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArchiveType",
                table: "ArchiveType");

            migrationBuilder.RenameTable(
                name: "ArchiveType",
                newName: "ArchiveTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArchiveTypes",
                table: "ArchiveTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Archives_ArchiveTypes_ArchiveTypeId",
                table: "Archives",
                column: "ArchiveTypeId",
                principalTable: "ArchiveTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Archives_ArchiveTypes_ArchiveTypeId",
                table: "Archives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArchiveTypes",
                table: "ArchiveTypes");

            migrationBuilder.RenameTable(
                name: "ArchiveTypes",
                newName: "ArchiveType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArchiveType",
                table: "ArchiveType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Archives_ArchiveType_ArchiveTypeId",
                table: "Archives",
                column: "ArchiveTypeId",
                principalTable: "ArchiveType",
                principalColumn: "Id");
        }
    }
}
