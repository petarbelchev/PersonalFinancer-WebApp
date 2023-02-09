using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class ChangedTablesNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_CurrencyTypes_CurrencyTypeId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "CurrencyTypes");

            migrationBuilder.RenameColumn(
                name: "CurrencyTypeId",
                table: "Accounts",
                newName: "CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_CurrencyTypeId",
                table: "Accounts",
                newName: "IX_Accounts_CurrencyId");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c990863-a80e-4d7e-b372-115fe0dceace",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "13af682f-79f6-4afe-a3e5-e472cfd77a24", "AQAAAAEAACcQAAAAEMng7VLFnu1mVZB2XvyBVpzqdKRk+jAZ2wyohTHeEdvFrtvsnri6PFJWvefi/spHJQ==", "345b0c88-3304-4f17-880c-1d8f3bfa2b90" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2e8ce625-1278-4368-87c5-9c79fd7692a4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0fca5902-f4e0-4a81-a7f4-c45804d2c695", "AQAAAAEAACcQAAAAEO8G0vGK0KvQgee/C8bcnm60PjuTcvBmfsgvMzgRo/9tg5hFer69pGrVusLKAtj1ZQ==", "3a6fcffc-d046-43bf-9ef3-02f6387b7df2" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "BGN" },
                    { 2, "EUR" },
                    { 3, "USD" }
                });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2023, 2, 8, 15, 58, 17, 766, DateTimeKind.Utc).AddTicks(4868));

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Currencies_CurrencyId",
                table: "Accounts",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Currencies_CurrencyId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "Accounts",
                newName: "CurrencyTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                newName: "IX_Accounts_CurrencyTypeId");

            migrationBuilder.CreateTable(
                name: "CurrencyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTypes", x => x.Id);
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
                table: "CurrencyTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "BGN" },
                    { 2, "EUR" },
                    { 3, "USD" }
                });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2023, 2, 8, 15, 39, 18, 926, DateTimeKind.Utc).AddTicks(1677));

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_CurrencyTypes_CurrencyTypeId",
                table: "Accounts",
                column: "CurrencyTypeId",
                principalTable: "CurrencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
