using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTypes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Refference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    { "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e", 0, "2967276f-6b14-4c8c-ad88-a76e5e3b918e", "petar@mail.com", false, "Petar", "Petrov", false, null, "PETAR@MAIL.COM", "PETAR@MAIL.COM", "AQAAAAEAACcQAAAAEI9k5VH8zCiPQNZ+Y1LDSlsgB+DW5L2HVnGnZrJMYf4PII3RofXUj+GCYRW+YWE0fg==", null, false, "d9b86a1c-62d8-4ffb-bd9d-7e83b02a8275", false, "petar@mail.com" },
                    { "bcb4f072-ecca-43c9-ab26-c060c6f364e4", 0, "4b1bbc1d-99e2-41e1-8371-8dc160018a0e", "teodor@mail.com", false, "Teodor", "Lesly", false, null, "TEODOR@MAIL.COM", "TEODOR@MAIL.COM", "AQAAAAEAACcQAAAAEFDapwAhzuqwd/Hew4GbQDIEKXXwMHdq3XH30lh0zmC2FVt7ZgFfjXl/bDClAXW9BA==", null, false, "8b4f5eb3-3a67-45f1-8e59-e7da4febaa1d", false, "teodor@mail.com" },
                    { "dea12856-c198-4129-b3f3-b893d8395082", 0, "ceb950c7-285e-48f7-9cae-50b7c61f8a08", "admin@admin.com", false, "Great", "Admin", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAENlLQtu94C2r3wF3RQh4N+JAx9WQTzjdNnxDXuSF07rZKZQfHa0LqNGfdOI3mD5djQ==", null, false, "ea899bcc-a166-44bc-849c-3c1856195267", false, "admin@admin.com" }
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
                    { new Guid("303430dc-63a3-4436-8907-a274ec29f608"), new Guid("1dfe1780-daed-4198-8360-378aa33c5411"), 1487.23m, new Guid("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"), false, "Bank USD", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), new Guid("1dfe1780-daed-4198-8360-378aa33c5411"), 900.01m, new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"), false, "Bank EUR", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("70169197-5c32-4430-ab39-34c776533376"), new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"), 825.71m, new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"), false, "Cash EUR", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), new Guid("1dfe1780-daed-4198-8360-378aa33c5411"), 2734.78m, new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"), false, "Bank BGN", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"), 189.55m, new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"), false, "Cash BGN", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CategoryId", "CreatedOn", "Refference", "TransactionType" },
                values: new object[,]
                {
                    { new Guid("0e613e98-0975-4ae9-b4f8-e44f8ddb51ad"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 5.65m, new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"), new DateTime(2023, 1, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2591), "Lunch", 1 },
                    { new Guid("0e750d8a-eeb9-45cc-bfd4-1221bf6aa651"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 4.80m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 2, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2619), "Taxi", 1 },
                    { new Guid("2803fee3-7f4b-49a9-bcf4-f5a2e7cdc2c7"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2694), "Initial Balance", 0 },
                    { new Guid("39809649-b514-445f-8702-de4691eeb9cb"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 24.29m, new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"), new DateTime(2023, 1, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2669), "Health Insurance", 1 },
                    { new Guid("3d782bdc-623f-42dd-954e-3b0a4e5de835"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 600m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2665), "Initial Balance", 0 },
                    { new Guid("52aaf8b7-e931-4153-b0b1-1a9a38c395ab"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 100.00m, new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"), new DateTime(2022, 12, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2636), "Electricity bill", 1 },
                    { new Guid("627ef639-493c-403c-b3b6-83b581e56e7d"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1834.78m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2628), "Initial Balance", 0 },
                    { new Guid("6767ad5f-ec08-4f8b-a4f7-0497112b8b1a"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 49.99m, new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"), new DateTime(2023, 1, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2703), "Flight ticket", 1 },
                    { new Guid("6f9047f6-a2ee-4ea9-8bc0-21df14812c76"), new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"), 750m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2699), "Salary", 0 },
                    { new Guid("9e2bbe87-4d69-4230-a4c5-50e5ad9821f9"), new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"), 1000.00m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 1, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2660), "Salary", 0 },
                    { new Guid("bc1883f4-bf24-4a32-af42-fbf158ea593d"), new Guid("70169197-5c32-4430-ab39-34c776533376"), 250m, new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"), new DateTime(2023, 3, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2688), "Salary", 0 },
                    { new Guid("e0fdd402-707a-427a-92e2-f8b00422e4ef"), new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"), 200m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 12, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(827), "Initial Balance", 0 },
                    { new Guid("e95fde53-a472-4754-9d9d-34aba479eae9"), new Guid("303430dc-63a3-4436-8907-a274ec29f608"), 1487.23m, new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"), new DateTime(2022, 7, 1, 8, 35, 3, 355, DateTimeKind.Utc).AddTicks(2708), "Initial Balance", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_OwnerId",
                table: "Accounts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_UserId",
                table: "AccountTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
