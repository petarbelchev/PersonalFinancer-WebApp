using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class UserNameSetUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5b55326d-4359-43cc-87ac-020190bd037b", "AQAAAAEAACcQAAAAEAvyvw3yLsNoddmlQFNrCyXYVwr3tw4FyWgb6a9NqObDII8HMymTByRjDS4yM6q/5g==", "faedf0c4-5a5d-46f6-b676-3df6b0008074" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c8da0524-4896-4f94-8b7d-decba2d9ed5d", "AQAAAAEAACcQAAAAEIc4sUh1KwbLMfdqzuZVRW7RtBhNFUBH9ioSHiN22HT89CSv1qRzNOpQzukTVxDFMA==", "eecfbb6e-7f71-46d1-9220-f4b2b48f9d69" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9722696-4596-4884-a4d3-0b615c4f56c7", "AQAAAAEAACcQAAAAEEkLIGo3ZOHw2HcWEm1YRHXjVyu4jmq6HoBLS5gRFYQmu8MZaQSOVS0stWUmE6WCfA==", "b23d03ec-29c8-4d52-89ed-802a1ff0ec98" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "33c1a3bb-c587-462c-a947-676193ab7e7c", "AQAAAAEAACcQAAAAEDE2b18osPUFCLCYOO918KGqw3yGoVX2OQzRpUn21xv5sbKMP7nMZyPp6lrnrO+YIA==", "1594438b-3b2e-4fac-8e5b-dbed2aff5f16" });
        }
    }
}
