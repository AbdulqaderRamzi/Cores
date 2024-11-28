using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations.HangfireDb
{
    /// <inheritdoc />
    public partial class InitHangfire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
