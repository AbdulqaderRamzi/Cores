using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequest_AspNetUsers_EmployeeID",
                table: "LeaveRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequest_LeaveTypes_LeaveTypeID",
                table: "LeaveRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequest",
                table: "LeaveRequest");

            migrationBuilder.RenameTable(
                name: "LeaveRequest",
                newName: "LeaveRequests");

            migrationBuilder.RenameColumn(
                name: "LeaveTypeID",
                table: "LeaveRequests",
                newName: "LeaveTypeId");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "LeaveRequests",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequest_LeaveTypeID",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequest_EmployeeID",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_EmployeeId");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LeaveRequests",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests",
                column: "LeaveTypeId",
                principalTable: "LeaveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequests",
                table: "LeaveRequests");

            migrationBuilder.RenameTable(
                name: "LeaveRequests",
                newName: "LeaveRequest");

            migrationBuilder.RenameColumn(
                name: "LeaveTypeId",
                table: "LeaveRequest",
                newName: "LeaveTypeID");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "LeaveRequest",
                newName: "EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_LeaveTypeId",
                table: "LeaveRequest",
                newName: "IX_LeaveRequest_LeaveTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_EmployeeId",
                table: "LeaveRequest",
                newName: "IX_LeaveRequest_EmployeeID");

            migrationBuilder.UpdateData(
                table: "LeaveRequest",
                keyColumn: "Reason",
                keyValue: null,
                column: "Reason",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LeaveRequest",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequest",
                table: "LeaveRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequest_AspNetUsers_EmployeeID",
                table: "LeaveRequest",
                column: "EmployeeID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequest_LeaveTypes_LeaveTypeID",
                table: "LeaveRequest",
                column: "LeaveTypeID",
                principalTable: "LeaveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
