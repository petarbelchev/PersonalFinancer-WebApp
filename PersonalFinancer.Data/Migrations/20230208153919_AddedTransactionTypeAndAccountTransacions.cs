using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class AddedTransactionTypeAndAccountTransacions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_CategoryTransactions_CategoryTransactionId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "CategoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CategoryTransactionId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "PaymentRefference",
                table: "Transactions",
                newName: "Refference");

            migrationBuilder.RenameColumn(
                name: "CategoryTransactionId",
                table: "Transactions",
                newName: "TransactionType");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c990863-a80e-4d7e-b372-115fe0dceace",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ff4e0b61-663a-4351-826e-577f5d9072df", "AQAAAAEAACcQAAAAEPj13OrWXhDJAWVbLzSuvdJ6aMtepi8peaoLgUeFljABikrOkdKxgY5cH+6d9yyRAw==", "6e920c38-11ad-4e48-8cf6-f299bc61c7b3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2e8ce625-1278-4368-87c5-9c79fd7692a4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dad2a4e2-8924-4a11-b30a-931bf67d83bb", "AQAAAAEAACcQAAAAENYsGKHxQBYcYGGPK8VIIOQ5kRqcHJntuXCqFEJzvWgOVis4MJFUrZGK70wD/VDpgw==", "2981b49c-8855-489c-8935-a0a6f9837e91" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, "Food & Drink", null },
                    { 2, "Utilities", null },
                    { 3, "Transportation", null },
                    { 4, "Housing", null },
                    { 5, "Medical & Healthcare", null }
                });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "CreatedOn", "TransactionType" },
                values: new object[] { 3, new DateTime(2023, 2, 8, 15, 39, 18, 926, DateTimeKind.Utc).AddTicks(1677), 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "CategoryTransactionId");

            migrationBuilder.RenameColumn(
                name: "Refference",
                table: "Transactions",
                newName: "PaymentRefference");

            migrationBuilder.CreateTable(
                name: "CategoryTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c990863-a80e-4d7e-b372-115fe0dceace",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f5bf43e-7f48-4089-bbb4-b4297c989618", "AQAAAAEAACcQAAAAEAtJYbUSq+0kvYabWB64RwhSAl/GZW+lRRjLisYWvNgnNaqFKWT4vcJvF5gIQb26rw==", "67920a4a-0f98-47b9-b4dd-2426b52e01bd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2e8ce625-1278-4368-87c5-9c79fd7692a4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c7925f05-2abf-4f07-809d-9fe36ebd4a07", "AQAAAAEAACcQAAAAEFRCunrlWMOaxNIzTvrPcISOVdyKt7UCh4ERNeMEvASLwCCf3al/q8cU4pF/SLbdDw==", "c1631380-f560-4278-8410-c9b4b352d089" });

            migrationBuilder.InsertData(
                table: "CategoryTransactions",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, "Food & Drink", null },
                    { 2, "Utilities", null },
                    { 3, "Transportation", null },
                    { 4, "Housing", null },
                    { 5, "Medical & Healthcare", null }
                });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryTransactionId", "CreatedOn" },
                values: new object[] { 3, new DateTime(2023, 2, 8, 9, 23, 49, 306, DateTimeKind.Utc).AddTicks(8075) });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryTransactionId",
                table: "Transactions",
                column: "CategoryTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTransactions_UserId",
                table: "CategoryTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_CategoryTransactions_CategoryTransactionId",
                table: "Transactions",
                column: "CategoryTransactionId",
                principalTable: "CategoryTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
