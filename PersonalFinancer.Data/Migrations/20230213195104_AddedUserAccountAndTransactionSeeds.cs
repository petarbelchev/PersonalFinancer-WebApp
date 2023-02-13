using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class AddedUserAccountAndTransactionSeeds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: new Guid("976799ef-5cec-4d65-b1bc-e6d0c4eedaa3"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: new Guid("e9fefdbf-b9e3-4924-9df7-1c0e1324b93f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("045b8079-068e-4dfa-9da4-9b0e9d0364a3"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("12af7c5f-1521-4a74-8563-121917b45cad"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("602d70b7-5857-4e2c-a32e-cc27e6a77940"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("8de5c04c-75b6-469c-8192-87c1c39beda4"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("8f99a43d-24c7-42e1-bfb4-030519393c3f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9c9a8b9d-a053-46b8-9e94-a0bd738e360f"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("010fe957-6059-4f39-bb75-575a4ea23e21"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("5e3ae38f-ecd9-44a7-9167-f07d4e40b47b"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("9b7253f8-8110-4fe0-8e18-94a209e52d1b"));

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "Id", "IsDeleted", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("1dfe1780-daed-4198-8360-378aa33c5411"), false, "Bank Account", null },
                    { new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"), false, "Cash", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e", 0, "69e911f4-86d9-4f13-b8b2-726c48920e1e", "petar@mail.com", false, "Petar", "Petrov", false, null, "PETAR@MAIL.COM", "PETAR@MAIL.COM", "AQAAAAEAACcQAAAAEA7zhLsa9jFugHdtgVkZJxLa5n0AwfWSwH5LzVcBzzsNHy0Lbzv89cfURvfOuaiXAQ==", null, false, "9980e9dc-b969-48a5-9097-69a3136c0ea6", false, "petar@mail.com" },
                    { "bcb4f072-ecca-43c9-ab26-c060c6f364e4", 0, "88d55fac-306c-4172-9555-5cdde466d9c2", "teodor@mail.com", false, "Teodor", "Lesly", false, null, "TEODOR@MAIL.COM", "TEODOR@MAIL.COM", "AQAAAAEAACcQAAAAEOEd1Md3PQF+zfiHP9qTSQFspaGZLSMk9kWnskVgQ6vJ9Zh7J1w2CDjHK+rOqgpscw==", null, false, "3b5d8ff1-029d-442b-8321-52154ca030c8", false, "teodor@mail.com" },
                    { "dea12856-c198-4129-b3f3-b893d8395082", 0, "91f74edb-a912-4866-822a-73c27e4c748e", "admin@admin.com", false, "Great", "Admin", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEDTxnmlHLR1+fw2ryxakvt0n8js1rY5XqmjSy11p4Oq1ROStxNSeuUpG6lBBhYYHiQ==", null, false, "dca664e6-c9a5-46c7-9770-1072b0c4b381", false, "admin@admin.com" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsDeleted", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), false, "Salary", null },
                    { new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), false, "Food & Drink", null },
                    { new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), false, "Medical & Healthcare", null },
                    { new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), false, "Transport", null },
                    { new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), false, "Utilities", null },
                    { new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), false, "Initial Balance", null }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"), "USD", null },
                    { new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"), "BGN", null },
                    { new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"), "EUR", null }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountTypeId", "Balance", "CurrencyId", "IsDeleted", "Name", "OwnerId" },
                values: new object[,]
                {
                    { new Guid("303430dc-63a3-4436-8907-a274ec29f608"), new Guid("1dfe1780-daed-4198-8360-378aa33c5411"), 200m, new Guid("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"), false, "Bank USD", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), new Guid("1dfe1780-daed-4198-8360-378aa33c5411"), 200m, new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"), false, "Bank EUR", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("70169197-5c32-4430-ab39-34c776533376"), new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"), 600m, new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"), false, "Cash EUR", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), new Guid("1dfe1780-daed-4198-8360-378aa33c5411"), 1834.78m, new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"), false, "Bank BGN", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"), 200m, new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"), false, "Cash BGN", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" }
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082");

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

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("303430dc-63a3-4436-8907-a274ec29f608"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("70169197-5c32-4430-ab39-34c776533376"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: new Guid("1dfe1780-daed-4198-8360-378aa33c5411"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"));

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "Id", "IsDeleted", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("976799ef-5cec-4d65-b1bc-e6d0c4eedaa3"), false, "Bank Account", null },
                    { new Guid("e9fefdbf-b9e3-4924-9df7-1c0e1324b93f"), false, "Cash", null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsDeleted", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("045b8079-068e-4dfa-9da4-9b0e9d0364a3"), false, "Medical & Healthcare", null },
                    { new Guid("12af7c5f-1521-4a74-8563-121917b45cad"), false, "Housing", null },
                    { new Guid("602d70b7-5857-4e2c-a32e-cc27e6a77940"), false, "Transport", null },
                    { new Guid("8de5c04c-75b6-469c-8192-87c1c39beda4"), false, "Initial Balance", null },
                    { new Guid("8f99a43d-24c7-42e1-bfb4-030519393c3f"), false, "Food & Drink", null },
                    { new Guid("9c9a8b9d-a053-46b8-9e94-a0bd738e360f"), false, "Utilities", null }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("010fe957-6059-4f39-bb75-575a4ea23e21"), "EUR", null },
                    { new Guid("5e3ae38f-ecd9-44a7-9167-f07d4e40b47b"), "BGN", null },
                    { new Guid("9b7253f8-8110-4fe0-8e18-94a209e52d1b"), "USD", null }
                });
        }
    }
}
