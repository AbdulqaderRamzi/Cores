using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddProblemTypeToProblemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "ProblemType",
                table: "Problems");

            migrationBuilder.AddColumn<int>(
                name: "ProblemTypeId",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0);*/

            /*migrationBuilder.CreateIndex(
                name: "IX_Problems_ProblemTypeId",
                table: "Problems",
                column: "ProblemTypeId");*/

            /*migrationBuilder.AddForeignKey(
                name: "FK_Problems_ProblemTypes_ProblemTypeId",
                table: "Problems",
                column: "ProblemTypeId",
                principalTable: "ProblemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_ProblemTypes_ProblemTypeId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ProblemTypeId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "ProblemTypeId",
                table: "Problems");

            migrationBuilder.AddColumn<string>(
                name: "ProblemType",
                table: "Problems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
