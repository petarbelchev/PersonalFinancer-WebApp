using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class UpdatedSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "9a78bb41-23fa-47fb-90a0-62907ab9e8f5", "PETAR2023", "AQAAAAEAACcQAAAAEKKcpPnikjIz8XFbqVXwactNB5DG/1Q3+6b4zChfzFFGQErJReaI/6a5p8BqJSwvHg==", "c4c98fb3-6ef7-4af0-b8ea-0b1a257eb082", "petar2023" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "205a00be-497b-400c-b6eb-a00762475c14", "ADMIN", "AQAAAAEAACcQAAAAELWShCXkxAcWe+S6VqGEjSzKnoB/Q8wgcPYDKq0ZbPmPcx2eW+0uXO56ztWpuf51LQ==", "c7a0d380-65a9-4417-84d6-820a36cf3de5", "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "5b55326d-4359-43cc-87ac-020190bd037b", "PETAR@MAIL.COM", "AQAAAAEAACcQAAAAEAvyvw3yLsNoddmlQFNrCyXYVwr3tw4FyWgb6a9NqObDII8HMymTByRjDS4yM6q/5g==", "faedf0c4-5a5d-46f6-b676-3df6b0008074", "petar@mail.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "c8da0524-4896-4f94-8b7d-decba2d9ed5d", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEIc4sUh1KwbLMfdqzuZVRW7RtBhNFUBH9ioSHiN22HT89CSv1qRzNOpQzukTVxDFMA==", "eecfbb6e-7f71-46d1-9220-f4b2b48f9d69", "admin@admin.com" });
        }
    }
}
