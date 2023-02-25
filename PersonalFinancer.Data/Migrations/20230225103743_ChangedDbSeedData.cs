using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class ChangedDbSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("36879b99-5f96-4863-b04e-e303fab4f387"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("399bea37-4f28-4bf8-a569-53bd5fc1ec33"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("5f5de9d7-dadb-40e0-8905-5a0d4df21027"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("8498f976-c824-49cf-bd9b-aa80a544f0fb"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("94f45fbd-c010-41e4-a5e5-e07e5e5712ee"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("997d20b7-f516-4502-8ed4-70d54abd759d"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("bf2a3f73-9f51-4c39-9825-88bad0042591"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("c28f1fcc-52f2-47f1-bcea-528c9827c89e"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("c474daee-0e6e-4629-96db-a12376dc07f8"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("c7cf89f8-1b16-46b7-bd37-43be0c0e1d08"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d3406ef6-5d07-46f8-81fb-6a7cdc71f517"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d4010356-2a6d-45bf-ab24-339348487a05"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d97d35f0-bc1b-43c4-80ea-792d972ac252"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("303430dc-63a3-4436-8907-a274ec29f608"),
                column: "Balance",
                value: 1487.23m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                column: "Balance",
                value: 900.01m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("70169197-5c32-4430-ab39-34c776533376"),
                column: "Balance",
                value: 825.71m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                column: "Balance",
                value: 2734.78m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                column: "Balance",
                value: 189.55m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7661beaf-3cc7-4ec1-ad93-2ac83cbd1a24", "AQAAAAEAACcQAAAAEMF0M9UNAZiUb8G5SUYJcH6M+yYRnk8dYU1FtZVvtfoo9frhcZMLjKQBI5ny3M+axw==", "24c415fc-431c-464b-a2b5-2bcb9aab401c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "818bd355-80b0-444b-b7cb-814e9060ec33", "AQAAAAEAACcQAAAAEOnBXRGToss+bHzfGUxZiM9y6YVo3kCBfOtBFC9dLF4uXoycKhQq/UsSTnveWi2b6g==", "e47fc12c-5581-472e-b712-c1cad22562c7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "06a84cfc-e2f5-4742-ae83-81d6cf94792c", "AQAAAAEAACcQAAAAEF+LD5WxVJT2wjyXNjJbWqE3RkxgxUQJu/Z2GIKsCwRKkjtoAdD8Bf0/VJOInyDKkQ==", "b7c3ffec-e5b5-4109-81ba-8ffcbd080e54" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[,]
                {
                    { new Guid("2161a16a-b825-4669-8706-3f733ec7bd29"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 600m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1743), "Initial Balance", 0 },
                    { new Guid("233ac3a9-79ad-4189-860e-6b6bd85443c1"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 5.65m, new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), new DateTime(2022, 12, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1713), "Lunch", 1 },
                    { new Guid("4b88a0a7-7ad3-48d0-bb98-c05a07e39d17"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 250m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 2, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1751), "Salary", 0 },
                    { new Guid("4c7a779c-2bde-47c3-b0ab-6d6b5e336864"), new Guid("303430dc-63a3-4436-8907-a274ec29f608"), 1487.23m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 6, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1765), "Initial Balance", 0 },
                    { new Guid("61a6484c-48f4-4229-b201-aff195604b67"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 4.80m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 1, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1718), "Taxi", 1 },
                    { new Guid("716080b0-8430-4110-a3db-2e20f0d89db8"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1000.00m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2022, 12, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1739), "Salary", 0 },
                    { new Guid("7e650b18-854d-4a07-900e-2178c7694ed4"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1701), "Initial Balance", 0 },
                    { new Guid("8cef1056-5a74-4513-b185-312fe6c00edd"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 24.29m, new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), new DateTime(2022, 12, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1747), "Health Insurance", 1 },
                    { new Guid("91aa8c08-24f8-4252-9a57-4dfce9c11a4d"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 750m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2022, 12, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1759), "Salary", 0 },
                    { new Guid("aad1b4e1-f9cd-43ed-bed5-abd8431d2f3d"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 49.99m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2022, 12, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1762), "Flight ticket", 1 },
                    { new Guid("ac0c1c0d-3ec1-47aa-8932-4a2e2ad1d32b"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1755), "Initial Balance", 0 },
                    { new Guid("d1214101-8695-4706-9a1d-3770bb499866"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 100.00m, new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), new DateTime(2022, 11, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1725), "Electricity bill", 1 },
                    { new Guid("f4e33e7a-55c7-4e41-8035-e49c185caabf"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1834.78m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 25, 10, 37, 43, 55, DateTimeKind.Utc).AddTicks(1721), "Initial Balance", 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("2161a16a-b825-4669-8706-3f733ec7bd29"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("233ac3a9-79ad-4189-860e-6b6bd85443c1"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("4b88a0a7-7ad3-48d0-bb98-c05a07e39d17"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("4c7a779c-2bde-47c3-b0ab-6d6b5e336864"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("61a6484c-48f4-4229-b201-aff195604b67"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("716080b0-8430-4110-a3db-2e20f0d89db8"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("7e650b18-854d-4a07-900e-2178c7694ed4"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("8cef1056-5a74-4513-b185-312fe6c00edd"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("91aa8c08-24f8-4252-9a57-4dfce9c11a4d"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("aad1b4e1-f9cd-43ed-bed5-abd8431d2f3d"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("ac0c1c0d-3ec1-47aa-8932-4a2e2ad1d32b"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d1214101-8695-4706-9a1d-3770bb499866"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("f4e33e7a-55c7-4e41-8035-e49c185caabf"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("303430dc-63a3-4436-8907-a274ec29f608"),
                column: "Balance",
                value: 200m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                column: "Balance",
                value: 200m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("70169197-5c32-4430-ab39-34c776533376"),
                column: "Balance",
                value: 600m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                column: "Balance",
                value: 1834.78m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                column: "Balance",
                value: 200m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "33259921-b57d-46f0-95dd-93a0eabe2148", "AQAAAAEAACcQAAAAEBaXk9U3fqQAaN9lA/0yDQXUZipRj5/GPsGtWAwIjfaNDdD0vRu+CemywdTwCZB/XA==", "7d6240eb-09e6-438c-87d1-c152fa8a6ec3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "69e4defc-4a1d-4bb3-a084-08e6624cfafb", "AQAAAAEAACcQAAAAEFymq9hycCs3lAXmQxUDMjr4r7RJ3wZBetmref4MYLRLs0T+T269tlujjJ1jx9lDmA==", "f1e2e607-1c37-4d72-a0e2-f533dee6a7bc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8590f857-6a75-42fa-959a-647988e3c79e", "AQAAAAEAACcQAAAAECuU1yJ68Q3JsGdOA9ISqe7Q5BuCZyiwKD16TG2pptl9vNvcNsl9ZLoqMWb/QqTEvQ==", "7680d0fd-7915-4785-a62b-0bf191fe2087" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[,]
                {
                    { new Guid("36879b99-5f96-4863-b04e-e303fab4f387"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2587), "Initial Balance", 0 },
                    { new Guid("399bea37-4f28-4bf8-a569-53bd5fc1ec33"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 4.80m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 1, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2547), "Taxi", 1 },
                    { new Guid("5f5de9d7-dadb-40e0-8905-5a0d4df21027"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 600m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2576), "Initial Balance", 0 },
                    { new Guid("8498f976-c824-49cf-bd9b-aa80a544f0fb"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 250m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 2, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2583), "Salary", 0 },
                    { new Guid("94f45fbd-c010-41e4-a5e5-e07e5e5712ee"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1000.00m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2573), "Salary", 0 },
                    { new Guid("997d20b7-f516-4502-8ed4-70d54abd759d"), new Guid("303430dc-63a3-4436-8907-a274ec29f608"), 1487.23m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 6, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2599), "Initial Balance", 0 },
                    { new Guid("bf2a3f73-9f51-4c39-9825-88bad0042591"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 100.00m, new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2567), "Electricity bill", 1 },
                    { new Guid("c28f1fcc-52f2-47f1-bcea-528c9827c89e"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 750m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2590), "Salary", 0 },
                    { new Guid("c474daee-0e6e-4629-96db-a12376dc07f8"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2531), "Initial Balance", 0 },
                    { new Guid("c7cf89f8-1b16-46b7-bd37-43be0c0e1d08"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 24.29m, new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2579), "Health Insurance", 1 },
                    { new Guid("d3406ef6-5d07-46f8-81fb-6a7cdc71f517"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 49.99m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2596), "Flight ticket", 1 },
                    { new Guid("d4010356-2a6d-45bf-ab24-339348487a05"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1834.78m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2564), "Initial Balance", 0 },
                    { new Guid("d97d35f0-bc1b-43c4-80ea-792d972ac252"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 5.65m, new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2544), "Lunch", 1 }
                });
        }
    }
}
