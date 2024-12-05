using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class MakeDepartmentHeadRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadId",
                table: "Departments");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentHeadId",
                keyValue: null,
                column: "DepartmentHeadId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentHeadId",
                table: "Departments",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadId",
                table: "Departments",
                column: "DepartmentHeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadId",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentHeadId",
                table: "Departments",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadId",
                table: "Departments",
                column: "DepartmentHeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
