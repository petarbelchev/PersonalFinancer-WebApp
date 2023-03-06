using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class AddedCurrencyIsDeletedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Currencies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "de3a1b07-ba36-4382-96bd-829fa8ad3a7f", "AQAAAAEAACcQAAAAENw6OB8QOqrb2KRgV57mAEhfp8lOnVtwRQBVsZC/0D6QVdGwD5pi5KtJVSUgRCfrAw==", "33efa086-c1b4-4667-9172-e8e7bf15cb4c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "40189e89-74f9-4801-b0c2-7f18b0cceff5", "AQAAAAEAACcQAAAAEKTy5WlBFxqBhaLsGuFMY2MAi+Ly4WwCRzKjPrnx+s5IM+wLYmjXXTYmzCLPo7oYRA==", "980d6103-85e3-40b2-9c16-adc7011fa5e6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dd451861-bb57-4fae-b456-1269a3c821d5", "AQAAAAEAACcQAAAAEOb8NPXFFE5Pod+Pib0yDDgQAYYFbKoIIYigfzxU9i4UL57t9egND3jvVdCDFAq4kA==", "082fa8e0-29e9-4d0a-8253-7e543e51914c" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[,]
                {
                    { new Guid("15808687-169e-4a36-92fa-73541f380b00"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 24.29m, new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), new DateTime(2023, 1, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7805), "Health Insurance", 1 },
                    { new Guid("2b1de6ea-f51a-4c2c-a9ab-0be839729b59"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 5.65m, new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), new DateTime(2023, 1, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7738), "Lunch", 1 },
                    { new Guid("376ddc0e-3bbe-4877-84c7-69554ab5a2c1"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 750m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7830), "Salary", 0 },
                    { new Guid("3eecdec1-0cb0-454f-ad12-e8b07dff783e"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 4.80m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 2, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7760), "Taxi", 1 },
                    { new Guid("4697990a-0961-402c-b04b-e6217e3f7ff6"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 100.00m, new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), new DateTime(2022, 12, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7773), "Electricity bill", 1 },
                    { new Guid("64898529-1ad6-4af5-a7b6-20f1760e6ba3"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1834.78m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7767), "Initial Balance", 0 },
                    { new Guid("9ce8a7cb-bd1d-432c-8b35-53d0efa51f87"), new Guid("303430dc-63a3-4436-8907-a274ec29f608"), 1487.23m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 7, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7840), "Initial Balance", 0 },
                    { new Guid("a2975953-baf0-432a-bf20-47fda7e6d68b"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1000.00m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7793), "Salary", 0 },
                    { new Guid("a8bdf863-446d-48f7-b137-cc1d740507f7"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(6039), "Initial Balance", 0 },
                    { new Guid("bb83316d-cbd4-4b0d-a646-0fa278e2b97a"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7825), "Initial Balance", 0 },
                    { new Guid("ddfa59a4-9dbc-4e42-811a-4643f1b242d5"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 250m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 3, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7811), "Salary", 0 },
                    { new Guid("e256d85c-d5a5-425e-9086-8323e5df2fa7"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 49.99m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 1, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7835), "Flight ticket", 1 },
                    { new Guid("ffb1ba13-66c1-43d4-9839-414b3c0d9a26"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 600m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 6, 17, 17, 58, 942, DateTimeKind.Utc).AddTicks(7799), "Initial Balance", 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("15808687-169e-4a36-92fa-73541f380b00"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("2b1de6ea-f51a-4c2c-a9ab-0be839729b59"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("376ddc0e-3bbe-4877-84c7-69554ab5a2c1"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("3eecdec1-0cb0-454f-ad12-e8b07dff783e"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("4697990a-0961-402c-b04b-e6217e3f7ff6"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("64898529-1ad6-4af5-a7b6-20f1760e6ba3"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("9ce8a7cb-bd1d-432c-8b35-53d0efa51f87"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("a2975953-baf0-432a-bf20-47fda7e6d68b"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("a8bdf863-446d-48f7-b137-cc1d740507f7"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("bb83316d-cbd4-4b0d-a646-0fa278e2b97a"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("ddfa59a4-9dbc-4e42-811a-4643f1b242d5"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("e256d85c-d5a5-425e-9086-8323e5df2fa7"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("ffb1ba13-66c1-43d4-9839-414b3c0d9a26"));

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Currencies");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c76165b7-53e1-4a0e-87f9-0ce10723d435", "AQAAAAEAACcQAAAAEOw5bk9YgVFzWWTr2In+zmKrW++b9xddzYgGbILM++zoZQlUIlvOS8n93K7GAN6nuA==", "de1f2be0-1828-4a03-ad3a-7bbe93e3a227" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2ddded61-9ed4-4a17-9bfc-e7fcebb953bd", "AQAAAAEAACcQAAAAEJC2ViPb8D2QodHkOLtYMhPAArBBsypZgKt5mZ5IwMHga7DRF0BpDzmzgOA/C4BKmg==", "976a3dd6-f95c-47e7-96af-01abfa829c03" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d59f93a5-22de-49ed-b86b-6e2564625aba", "AQAAAAEAACcQAAAAEI0yfR6p9p0OJge9hyDBRT4GhsTmb/V19LyYpRpN47scduMIwJTr6cvCMQusRrp+kg==", "e074a0e7-e105-4445-a749-83a4b9efc470" });

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
    }
}
