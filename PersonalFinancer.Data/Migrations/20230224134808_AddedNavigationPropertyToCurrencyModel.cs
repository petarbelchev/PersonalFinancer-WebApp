using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class AddedNavigationPropertyToCurrencyModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("1bdbe330-f3d4-400e-ab54-6979c351b4a5"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("2dee6e08-c979-421a-8cc6-c3fb88f015bb"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("349866c9-e031-4ada-9698-5d508dffb8e7"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("486408ac-0cfc-49f3-9f4d-282a6e580ec2"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("494e4c81-246f-43e9-b3de-82616c125b78"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("71ff0e89-4269-412f-b2c2-e3be846afe21"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("a2a351de-a2ec-4075-82db-353953ca4106"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("a8923db0-36c9-4378-92a2-1de8c039ad28"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("aa59ad99-17a9-405a-954e-3d6489e343d8"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("c0f25f9e-9dfd-40bd-9521-71013f3407f3"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("dbc148a4-de06-4616-b04e-9197cd01ed4c"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("f38a72cc-a8b2-4e74-b7d5-25c86df9a4e0"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("fbd23af4-6771-46bf-a2f0-157bb23ef691"));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Currencies",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_AspNetUsers_UserId",
                table: "Currencies",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_AspNetUsers_UserId",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies");

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

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Currencies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "69e911f4-86d9-4f13-b8b2-726c48920e1e", "AQAAAAEAACcQAAAAEA7zhLsa9jFugHdtgVkZJxLa5n0AwfWSwH5LzVcBzzsNHy0Lbzv89cfURvfOuaiXAQ==", "9980e9dc-b969-48a5-9097-69a3136c0ea6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "88d55fac-306c-4172-9555-5cdde466d9c2", "AQAAAAEAACcQAAAAEOEd1Md3PQF+zfiHP9qTSQFspaGZLSMk9kWnskVgQ6vJ9Zh7J1w2CDjHK+rOqgpscw==", "3b5d8ff1-029d-442b-8321-52154ca030c8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "91f74edb-a912-4866-822a-73c27e4c748e", "AQAAAAEAACcQAAAAEDTxnmlHLR1+fw2ryxakvt0n8js1rY5XqmjSy11p4Oq1ROStxNSeuUpG6lBBhYYHiQ==", "dca664e6-c9a5-46c7-9770-1072b0c4b381" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[,]
                {
                    { new Guid("1bdbe330-f3d4-400e-ab54-6979c351b4a5"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 750m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2022, 12, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4647), "Salary", 0 },
                    { new Guid("2dee6e08-c979-421a-8cc6-c3fb88f015bb"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4636), "Initial Balance", 0 },
                    { new Guid("349866c9-e031-4ada-9698-5d508dffb8e7"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1000.00m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2022, 12, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4614), "Salary", 0 },
                    { new Guid("486408ac-0cfc-49f3-9f4d-282a6e580ec2"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4557), "Initial Balance", 0 },
                    { new Guid("494e4c81-246f-43e9-b3de-82616c125b78"), new Guid("303430dc-63a3-4436-8907-a274ec29f608"), 1487.23m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 6, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4664), "Initial Balance", 0 },
                    { new Guid("71ff0e89-4269-412f-b2c2-e3be846afe21"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 4.80m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 1, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4594), "Taxi", 1 },
                    { new Guid("a2a351de-a2ec-4075-82db-353953ca4106"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 100.00m, new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), new DateTime(2022, 11, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4605), "Electricity bill", 1 },
                    { new Guid("a8923db0-36c9-4378-92a2-1de8c039ad28"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 24.29m, new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), new DateTime(2022, 12, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4624), "Health Insurance", 1 },
                    { new Guid("aa59ad99-17a9-405a-954e-3d6489e343d8"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 600m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4619), "Initial Balance", 0 },
                    { new Guid("c0f25f9e-9dfd-40bd-9521-71013f3407f3"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 5.65m, new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), new DateTime(2022, 12, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4574), "Lunch", 1 },
                    { new Guid("dbc148a4-de06-4616-b04e-9197cd01ed4c"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 250m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 2, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4629), "Salary", 0 },
                    { new Guid("f38a72cc-a8b2-4e74-b7d5-25c86df9a4e0"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 49.99m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2022, 12, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4656), "Flight ticket", 1 },
                    { new Guid("fbd23af4-6771-46bf-a2f0-157bb23ef691"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1834.78m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 11, 13, 19, 51, 3, 561, DateTimeKind.Utc).AddTicks(4601), "Initial Balance", 0 }
                });
        }
    }
}
