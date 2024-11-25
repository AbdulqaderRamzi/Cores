using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cores.DataService.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceAccountingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AccountTypes_AccountTypeId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_Accounts_AccountId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_AspNetUsers_ApplicationUserId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Journals_JournalTypes_JournalTypeId",
                table: "Journals");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_CreditAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_DebitAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_ApplicationUserId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "JournalTypes");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ApplicationUserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CreditAccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_DebitAccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Journals_JournalTypeId",
                table: "Journals");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_AccountId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_ApplicationUserId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreditAccountId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DebitAccountId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "JournalTypeId",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "Debit",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "AccountTypeId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Transactions",
                newName: "TransactionDate");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "TotalDebit");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "JournalEntries",
                newName: "EntryDate");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Accounts",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Transactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "Transactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Transactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCredit",
                table: "Transactions",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Journals",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Journals",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Journals",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Journals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Journals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Journals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Journals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "JournalEntries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "JournalEntries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EntryNumber",
                table: "JournalEntries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsPosted",
                table: "JournalEntries",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PostedBy",
                table: "JournalEntries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedDate",
                table: "JournalEntries",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Accounts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Accounts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "JournalEntryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    DebitAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntryDetails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryDetails_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    DebitAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionDetails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionDetails_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryDetails_AccountId",
                table: "JournalEntryDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryDetails_JournalEntryId",
                table: "JournalEntryDetails",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_AccountId",
                table: "TransactionDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_TransactionId",
                table: "TransactionDetails",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalEntryDetails");

            migrationBuilder.DropTable(
                name: "TransactionDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TotalCredit",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "EntryNumber",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "IsPosted",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "PostedBy",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "PostedDate",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "Transactions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "TotalDebit",
                table: "Transactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "EntryDate",
                table: "JournalEntries",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Accounts",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Transactions",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CreditAccountId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DebitAccountId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Journals",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Journals",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JournalTypeId",
                table: "Journals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "JournalEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "JournalEntries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "JournalEntries",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "JournalEntries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Debit",
                table: "JournalEntries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AccountTypeId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ApplicationUserId",
                table: "Transactions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreditAccountId",
                table: "Transactions",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DebitAccountId",
                table: "Transactions",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_JournalTypeId",
                table: "Journals",
                column: "JournalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_AccountId",
                table: "JournalEntries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_ApplicationUserId",
                table: "JournalEntries",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AccountTypes_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId",
                principalTable: "AccountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_Accounts_AccountId",
                table: "JournalEntries",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_AspNetUsers_ApplicationUserId",
                table: "JournalEntries",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Journals_JournalTypes_JournalTypeId",
                table: "Journals",
                column: "JournalTypeId",
                principalTable: "JournalTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_CreditAccountId",
                table: "Transactions",
                column: "CreditAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_DebitAccountId",
                table: "Transactions",
                column: "DebitAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_ApplicationUserId",
                table: "Transactions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
