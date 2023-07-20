#nullable disable

namespace PersonalFinancer.Data.Migrations
{
	using Microsoft.EntityFrameworkCore.Migrations;

	public partial class RenameDateTimeProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Transactions",
                newName: "CreatedOnUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Transactions",
                newName: "CreatedOn");
        }
    }
}
