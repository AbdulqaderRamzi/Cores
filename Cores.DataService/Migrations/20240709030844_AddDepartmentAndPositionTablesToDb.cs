using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentAndPositionTablesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadID",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "DepartmentHeadID",
                table: "Departments",
                newName: "DepartmentHeadId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_DepartmentHeadID",
                table: "Departments",
                newName: "IX_Departments_DepartmentHeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadId",
                table: "Departments",
                column: "DepartmentHeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadId",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "DepartmentHeadId",
                table: "Departments",
                newName: "DepartmentHeadID");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_DepartmentHeadId",
                table: "Departments",
                newName: "IX_Departments_DepartmentHeadID");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_DepartmentHeadID",
                table: "Departments",
                column: "DepartmentHeadID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
