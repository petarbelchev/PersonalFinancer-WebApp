using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class RemoveSeededAccountsAndUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c990863-a80e-4d7e-b372-115fe0dceace");

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2e8ce625-1278-4368-87c5-9c79fd7692a4");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Initial Balance");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Food & Drink");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Utilities");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Transportation");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Housing");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[] { 6, "Medical & Healthcare", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "2c990863-a80e-4d7e-b372-115fe0dceace", 0, "50bcd396-6f02-47bb-878b-291d35cc3a24", "petar_hristov@mail.com", false, "Petar", "Hristov", false, null, "PETAR_HRISTOV@MAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEHBNhYHQTVqHKFOb/zc7pjqPXvm+p9/+L7el6+hoF+hsMepgCL/43O9fvKSl/MC22g==", "+359111111111", false, "b0d168d5-ef6e-4ffa-aef4-f98102d8aae3", false, "admin" },
                    { "2e8ce625-1278-4368-87c5-9c79fd7692a4", 0, "82127258-32d6-4849-ae6c-912fd3fef91c", "ivan.ivanov@abv.bg", false, "Ivan", "Ivanov", false, null, "IVAN.IVANOV@ABV.BG", "REGULARUSER", "AQAAAAEAACcQAAAAELaS8AaLTGY5ZARuDTVy1BqCQ3ZvTIIw569hxXQOesy4+qHf0HJlF6R+JS0XBMLCZg==", "+359222222222", false, "134b0098-8cde-4d7c-b3e7-04a5595b6846", false, "regularUser" }
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Food & Drink");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Utilities");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Transportation");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Housing");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Medical & Healthcare");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountTypeId", "Balance", "CurrencyId", "Name", "OwnerId" },
                values: new object[] { 1, 1, 100.00m, 1, "MyCashMoney", "2e8ce625-1278-4368-87c5-9c79fd7692a4" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountTypeId", "Balance", "CurrencyId", "Name", "OwnerId" },
                values: new object[] { 2, 2, 1500.00m, 2, "MyBankMoney", "2e8ce625-1278-4368-87c5-9c79fd7692a4" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[] { 1, 1, 25m, 3, new DateTime(2023, 2, 8, 16, 50, 8, 376, DateTimeKind.Utc).AddTicks(1082), "My first transport transaction.", 0 });
        }
    }
}
