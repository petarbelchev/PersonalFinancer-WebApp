using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class UpdatedUsersDataSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "c76165b7-53e1-4a0e-87f9-0ce10723d435", "AQAAAAEAACcQAAAAEOw5bk9YgVFzWWTr2In+zmKrW++b9xddzYgGbILM++zoZQlUIlvOS8n93K7GAN6nuA==", "1234567890", "de1f2be0-1828-4a03-ad3a-7bbe93e3a227" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "2ddded61-9ed4-4a17-9bfc-e7fcebb953bd", "AQAAAAEAACcQAAAAEJC2ViPb8D2QodHkOLtYMhPAArBBsypZgKt5mZ5IwMHga7DRF0BpDzmzgOA/C4BKmg==", "1325476980", "976a3dd6-f95c-47e7-96af-01abfa829c03" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "d59f93a5-22de-49ed-b86b-6e2564625aba", "AQAAAAEAACcQAAAAEI0yfR6p9p0OJge9hyDBRT4GhsTmb/V19LyYpRpN47scduMIwJTr6cvCMQusRrp+kg==", "9876543021", "e074a0e7-e105-4445-a749-83a4b9efc470" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[,]
                {
                    { new Guid("122668d4-a574-4458-b679-ce9f0f769600"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6660), "Initial Balance", 0 },
                    { new Guid("1b690de4-3e83-4eb1-b895-1a5d4c160de4"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(4880), "Initial Balance", 0 },
                    { new Guid("37b8887f-2ccb-4bca-8554-9da6989aaba7"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 250m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 3, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6653), "Salary", 0 },
                    { new Guid("67e512b3-b22a-4cd6-8416-8c76b00aca9a"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 100.00m, new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), new DateTime(2022, 12, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6622), "Electricity bill", 1 },
                    { new Guid("6e3376d8-8c68-4307-8921-6416afb81932"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1000.00m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6637), "Salary", 0 },
                    { new Guid("7ba96879-cb14-4c40-94f5-b71b43f58b02"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1834.78m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6617), "Initial Balance", 0 },
                    { new Guid("92926aa2-870b-424a-8bab-9072d81b0887"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 4.80m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 2, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6610), "Taxi", 1 },
                    { new Guid("9bdd7f9b-ae32-402a-ac59-33ed077a5f6d"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 600m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6642), "Initial Balance", 0 },
                    { new Guid("a448aebc-4317-4873-a3c9-afdd0ea83a92"), new Guid("303430dc-63a3-4436-8907-a274ec29f608"), 1487.23m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 7, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6679), "Initial Balance", 0 },
                    { new Guid("cac5aa32-86c8-471a-8e65-9cba3a6e9a3e"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 49.99m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 1, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6673), "Flight ticket", 1 },
                    { new Guid("cce2bdf4-a23a-4960-a4cd-dd641a7fbb66"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 5.65m, new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), new DateTime(2023, 1, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6578), "Lunch", 1 },
                    { new Guid("ee3f171c-2479-4abe-ae26-56fb9fc19998"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 750m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6668), "Salary", 0 },
                    { new Guid("f40fb7ff-e8a3-46ca-ae83-dd444043f0e3"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 24.29m, new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), new DateTime(2023, 1, 2, 15, 54, 17, 58, DateTimeKind.Utc).AddTicks(6648), "Health Insurance", 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("122668d4-a574-4458-b679-ce9f0f769600"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("1b690de4-3e83-4eb1-b895-1a5d4c160de4"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("37b8887f-2ccb-4bca-8554-9da6989aaba7"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("67e512b3-b22a-4cd6-8416-8c76b00aca9a"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("6e3376d8-8c68-4307-8921-6416afb81932"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("7ba96879-cb14-4c40-94f5-b71b43f58b02"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("92926aa2-870b-424a-8bab-9072d81b0887"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("9bdd7f9b-ae32-402a-ac59-33ed077a5f6d"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("a448aebc-4317-4873-a3c9-afdd0ea83a92"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("cac5aa32-86c8-471a-8e65-9cba3a6e9a3e"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("cce2bdf4-a23a-4960-a4cd-dd641a7fbb66"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("ee3f171c-2479-4abe-ae26-56fb9fc19998"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("f40fb7ff-e8a3-46ca-ae83-dd444043f0e3"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "7661beaf-3cc7-4ec1-ad93-2ac83cbd1a24", "AQAAAAEAACcQAAAAEMF0M9UNAZiUb8G5SUYJcH6M+yYRnk8dYU1FtZVvtfoo9frhcZMLjKQBI5ny3M+axw==", null, "24c415fc-431c-464b-a2b5-2bcb9aab401c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "818bd355-80b0-444b-b7cb-814e9060ec33", "AQAAAAEAACcQAAAAEOnBXRGToss+bHzfGUxZiM9y6YVo3kCBfOtBFC9dLF4uXoycKhQq/UsSTnveWi2b6g==", null, "e47fc12c-5581-472e-b712-c1cad22562c7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "06a84cfc-e2f5-4742-ae83-81d6cf94792c", "AQAAAAEAACcQAAAAEF+LD5WxVJT2wjyXNjJbWqE3RkxgxUQJu/Z2GIKsCwRKkjtoAdD8Bf0/VJOInyDKkQ==", null, "b7c3ffec-e5b5-4109-81ba-8ffcbd080e54" });

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
    }
}
