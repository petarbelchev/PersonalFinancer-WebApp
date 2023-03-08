using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class UpdatedModelsValidations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccountTypes",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e1909cba-6a0e-4956-a5ed-961608230294", "AQAAAAEAACcQAAAAEG99NnwggvkIPSDY2wBYtMT+t8/duvhPkAdZFfXGRxxCNYuOp2P3FKbScDtQ6PUyGg==", "593c7b8c-8d60-4a78-a24e-932670de17bd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3e503f98-b332-4998-aeef-2e97d017473f", "AQAAAAEAACcQAAAAEIb4Isv5P5X7s9YPqW+iHWhNG/K/ovLIjbofSv6KZIeYGxaW8XpTyKsDjXQkfblnXA==", "98d90ce9-de1a-426b-9f9d-b34bbf16df0c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bde8d3fd-30de-4807-92aa-4abbb087fbd3", "AQAAAAEAACcQAAAAEMVIK90e48owRodrR9m89jaR2BkqxnorXJKYUIpDi52MYiUDk/XXoSVqe7YzMUS+sQ==", "bae940df-11d0-4a94-8102-27c5cf37cd42" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[,]
                {
                    { new Guid("14895d28-597f-452a-adc6-a03cb817bd30"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 750m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7043), "Salary", 0 },
                    { new Guid("1a8faa14-e3e8-4f06-be52-e92c934380d1"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 600m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7004), "Initial Balance", 0 },
                    { new Guid("1f1ea777-03e7-4176-8b65-69f3cb3c779f"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 4.80m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 2, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6975), "Taxi", 1 },
                    { new Guid("2ce5f0dd-1bbe-4a44-b12e-485e5a0d7209"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1000.00m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6999), "Salary", 0 },
                    { new Guid("2d11697d-b0b9-4b7e-a285-e6968d2415ef"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 5.65m, new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6956), "Lunch", 1 },
                    { new Guid("3ee36d75-1c8b-4945-9836-73f90bcf5be7"), new Guid("303430dc-63a3-4436-8907-a274ec29f608"), 1487.23m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 7, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7053), "Initial Balance", 0 },
                    { new Guid("481e697e-4b27-4163-92a8-c3d48d943898"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1834.78m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6980), "Initial Balance", 0 },
                    { new Guid("56e72652-cb1e-4c80-ba61-74be466a49d6"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7039), "Initial Balance", 0 },
                    { new Guid("7661ddfb-257c-4c3a-a02f-89f6541a0c06"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(5463), "Initial Balance", 0 },
                    { new Guid("96152eaa-c751-417a-880a-8cbea3815fd9"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 250m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 3, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7032), "Salary", 0 },
                    { new Guid("d243b590-117b-4b04-a831-4d983dfc4302"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 49.99m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7047), "Flight ticket", 1 },
                    { new Guid("e9a40bde-668c-44cb-a281-1c990299ed5c"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 24.29m, new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7027), "Health Insurance", 1 },
                    { new Guid("ebaa4859-647a-44c5-9630-b9379534cf4b"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 100.00m, new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6986), "Electricity bill", 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("14895d28-597f-452a-adc6-a03cb817bd30"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("1a8faa14-e3e8-4f06-be52-e92c934380d1"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("1f1ea777-03e7-4176-8b65-69f3cb3c779f"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("2ce5f0dd-1bbe-4a44-b12e-485e5a0d7209"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("2d11697d-b0b9-4b7e-a285-e6968d2415ef"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("3ee36d75-1c8b-4945-9836-73f90bcf5be7"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("481e697e-4b27-4163-92a8-c3d48d943898"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("56e72652-cb1e-4c80-ba61-74be466a49d6"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("7661ddfb-257c-4c3a-a02f-89f6541a0c06"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("96152eaa-c751-417a-880a-8cbea3815fd9"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d243b590-117b-4b04-a831-4d983dfc4302"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("e9a40bde-668c-44cb-a281-1c990299ed5c"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("ebaa4859-647a-44c5-9630-b9379534cf4b"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccountTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

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
    }
}
