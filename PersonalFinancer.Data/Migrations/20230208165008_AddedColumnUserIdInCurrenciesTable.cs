using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class AddedColumnUserIdInCurrenciesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Currencies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c990863-a80e-4d7e-b372-115fe0dceace",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "50bcd396-6f02-47bb-878b-291d35cc3a24", "AQAAAAEAACcQAAAAEHBNhYHQTVqHKFOb/zc7pjqPXvm+p9/+L7el6+hoF+hsMepgCL/43O9fvKSl/MC22g==", "b0d168d5-ef6e-4ffa-aef4-f98102d8aae3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2e8ce625-1278-4368-87c5-9c79fd7692a4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "82127258-32d6-4849-ae6c-912fd3fef91c", "AQAAAAEAACcQAAAAELaS8AaLTGY5ZARuDTVy1BqCQ3ZvTIIw569hxXQOesy4+qHf0HJlF6R+JS0XBMLCZg==", "134b0098-8cde-4d7c-b3e7-04a5595b6846" });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2023, 2, 8, 16, 50, 8, 376, DateTimeKind.Utc).AddTicks(1082));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Currencies");

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

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2023, 2, 8, 15, 58, 17, 766, DateTimeKind.Utc).AddTicks(4868));
        }
    }
}
