using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class AddedInitialBalanceProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInitialBalance",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bfadd2e3-78e8-4cfc-bccd-d0ff8391fe88", "AQAAAAEAACcQAAAAELGy0//ZGonjIRHp/z+entuedLSYewhacbeV+g6hUkAzy3kfwVEjYjt9OdFBVq5x0g==", "bc754d75-e49f-4187-850d-d7409d442ddb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e31b2856-2b03-42b7-b90d-9ba03569bb43", "AQAAAAEAACcQAAAAEJxDs/AmIyUt+GwHt0Sd30cLmg8tPLbJpE6t1tirenT9x62zPUSbVu6BvRd9+0sa0A==", "87bde630-338d-4446-a725-e5516a4fa8a2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4d33b698-39e3-405b-b0a3-003f724ba567", "AQAAAAEAACcQAAAAEOYhj0o1lu0mxSI/mP9ZvLd5sCaJJa+JQ9E1CTM3RK7sJFsv4GhVPyDjCeUMtM8Rfg==", "b77b1d5d-f756-4f18-81b8-cfb0fdda9ff8" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInitialBalance",
                table: "Transactions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "114af31f-a596-420f-9617-7e7408dd0588", "AQAAAAEAACcQAAAAEFhKQl+SeJMcQoUiwuYpGd/B19vinbTwpvbgs31MRu9yFm3bFQpq1f6WYzZWDfdClQ==", "239b553c-d6c2-4f02-86d1-2c008e8facfb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c34b5c2-2b9e-488f-adf6-6195b4ace5fe", "AQAAAAEAACcQAAAAEAscUNLE32Xa5P/zJ9ELf3KnsUjrhkcgqNLlDw8VV52cJ54fL7rOrot5sVGv18me1w==", "8fdeaf27-21c4-43a7-8971-9a2db7b91caa" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b6eb14c-8f3d-49e2-951f-e0240dfa68f0", "AQAAAAEAACcQAAAAEMyJ+2Z18z8GN+mT4Q3KCqx4ADbUduWm7fvUhx46gkQgNgCnAiWGIbkORbGxXDAMtQ==", "e99955e6-fb2a-44f2-a082-740a7187eff2" });
        }
    }
}
