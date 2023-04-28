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
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTypes_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrencyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Refference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsInitialBalance = table.Column<bool>(type: "bit", nullable: false)
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
                        name: "FK_Transactions_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "47af2a07-4a22-444e-bc1c-b4f7e37e09f7", 0, "5c082d16-5a64-4d7d-a839-0bc9f8de7e42", "user11@mail.com", false, "User11", "Userov11", false, null, "USER11@MAIL.COM", "USER11@MAIL.COM", "AQAAAAEAACcQAAAAEGLHkQohO//1fv5fMH5U3yKyvxBiW4BuNNrlazwzamJMHyypit3sGZwOoTUyVVraKw==", "1325476911", false, "21cd09f9-64c4-4c44-9c5c-e39a7aeacb72", false, "user11@mail.com" },
                    { "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e", 0, "4ccd49a4-02b1-410f-871f-d8f6f4cb7b92", "petar@mail.com", false, "Petar", "Petrov", false, null, "PETAR@MAIL.COM", "PETAR@MAIL.COM", "AQAAAAEAACcQAAAAENLI/KO6bpBuEzI5vjuhBC9iNRSddqorQfq421fS3Q3SNu0vIg/UILuW1X7WieuWdA==", "1234567890", false, "fcd4ab59-a6ed-456f-8e3e-d256600b90e5", false, "petar@mail.com" },
                    { "71b67705-e8d3-4862-9ad2-b76ac7ef92b0", 0, "82c4eb33-b259-49d4-9bdb-d2dc0817cd3c", "user6@mail.com", false, "User6", "Userov6", false, null, "USER6@MAIL.COM", "USER6@MAIL.COM", "AQAAAAEAACcQAAAAEADzquUUChMv5injYmNo+ob9CuzMwjXr22GQIFrWnSKDPYLT/ZUGM8ReXjzD1ncCrA==", "132547696", false, "abe84cd8-c1ef-4479-b99d-93416e8e2178", false, "user6@mail.com" },
                    { "8622144f-8888-4a4b-a7ef-bc4c71026f85", 0, "397f3d28-d4b2-4d9c-ad25-e891c040ee5a", "user10@mail.com", false, "User10", "Userov10", false, null, "USER10@MAIL.COM", "USER10@MAIL.COM", "AQAAAAEAACcQAAAAEAaDGnTC0Hh/eYOZbzISIIodJ/lXruSRyOXUlYoL/MP38uYAeEb2nrdxrQ+2fOJydA==", "1325476910", false, "92a8c587-8c0a-4427-baf7-02fc0c29213c", false, "user10@mail.com" },
                    { "87c9de28-42f0-44b6-9108-ff069989d744", 0, "c476750b-4d16-49d5-ae42-cd70d0071bcd", "user5@mail.com", false, "User5", "Userov5", false, null, "USER5@MAIL.COM", "USER5@MAIL.COM", "AQAAAAEAACcQAAAAEM3xEutqOtxQESh5UQv5+rREzGyckgvlc8NHJleTVIq/H+KB/qNlO26DIA4ujzPDCA==", "132547695", false, "9c8f78b4-b750-458f-b605-720783113c6e", false, "user5@mail.com" },
                    { "94190cf7-20a0-4c82-a6d2-ceebf165e0f0", 0, "65abda39-399c-47fe-82de-de9977030ae6", "user7@mail.com", false, "User7", "Userov7", false, null, "USER7@MAIL.COM", "USER7@MAIL.COM", "AQAAAAEAACcQAAAAEM2BRqxReiC++NigAupvbejLkMdceatr2o0sF8tFUFDgA1xbx6zzPgGVh59vBpBPmQ==", "132547697", false, "b9d7b2e1-dcfa-4457-bc4b-a6475f099c06", false, "user7@mail.com" },
                    { "9b69aa77-5f40-4ef8-87be-9eb3d53f6bc7", 0, "1e0cdd73-8a79-466b-9b62-e49b664d9272", "user13@mail.com", false, "User13", "Userov13", false, null, "USER13@MAIL.COM", "USER13@MAIL.COM", "AQAAAAEAACcQAAAAEEK8pZaBahtagPe5FfmUJ3yeTPvyaar5S/x9OhSrw1sPKQBxe78SE8nt/7X02oEICA==", "1325476913", false, "2f4ebcf4-3cfa-4d83-a775-ccdd82c25238", false, "user13@mail.com" },
                    { "a613ace1-0af7-4ec7-b663-cb4b9fd19e51", 0, "46ce3012-1a0d-41c8-a205-50bb91a360be", "user8@mail.com", false, "User8", "Userov8", false, null, "USER8@MAIL.COM", "USER8@MAIL.COM", "AQAAAAEAACcQAAAAENG/kCTyVIYkuJhJ+tXE6siQ+UDkrbAKqgppdo0OYDVgtQ6bMVfh6tHYUx8ngv2cdg==", "132547698", false, "7c9faaf5-f389-4b4e-8637-1514a08bbb5e", false, "user8@mail.com" },
                    { "a9573f5f-ffd5-4755-9e4b-f36d3325d14c", 0, "dc3a5e51-7dec-4843-a2dc-98e25b6afb57", "user12@mail.com", false, "User12", "Userov12", false, null, "USER12@MAIL.COM", "USER12@MAIL.COM", "AQAAAAEAACcQAAAAEOI9r9hHEvxz9rn9zQnqMAAAgu8qBaY0KbigFEhsklb9IPzXfBo++P8Yn65/WLfQJg==", "1325476912", false, "9e764b77-3832-4183-aa86-06d6af3e23af", false, "user12@mail.com" },
                    { "bcb4f072-ecca-43c9-ab26-c060c6f364e4", 0, "6c6707c4-e7ea-4222-9eb6-6bd830cd831f", "teodor@mail.com", false, "Teodor", "Lesly", false, null, "TEODOR@MAIL.COM", "TEODOR@MAIL.COM", "AQAAAAEAACcQAAAAEE7LZzIGTn6eV361Hf9SfKW21mdSlVzAmQo8uOapIK1jEFry/Sl4l3FWyXDpL11BRA==", "1325476980", false, "ad90f9b8-3617-4db8-a2b3-e73e5b17c218", false, "teodor@mail.com" },
                    { "c179a9af-914d-45db-ac99-ac1e4fa1a81e", 0, "4a527089-80bb-44f7-aead-8e7131c46ce8", "user9@mail.com", false, "User9", "Userov9", false, null, "USER9@MAIL.COM", "USER9@MAIL.COM", "AQAAAAEAACcQAAAAEB1ayeIjavs/CLXkglkiiRBCXAGXkCqp24XgKDZ9zmEyyOxWFt0F8TMYLwK/FrmWDQ==", "132547699", false, "d7358981-6ba0-4632-a6e5-a41d9a740330", false, "user9@mail.com" },
                    { "d75dbfb1-b1e9-4599-8267-d169c29a1356", 0, "24509da9-4db3-4a50-ac5c-a04fb9d89602", "user4@mail.com", false, "User4", "Userov4", false, null, "USER4@MAIL.COM", "USER4@MAIL.COM", "AQAAAAEAACcQAAAAELR3rVO2WumNwu7MxwMunh6KTdSRW6m5DDAns8IEHYzgRgMEmCzah4Iajk2uqiQZTw==", "132547694", false, "a9ce9d32-4b3e-489c-bab5-ff44899bfc1d", false, "user4@mail.com" },
                    { "dea12856-c198-4129-b3f3-b893d8395082", 0, "e03d247e-de84-4c96-acfc-a5c543c944a3", "admin@admin.com", false, "Great", "Admin", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAELG0xnD1pq64QMb9ItDNiPeWe+SdNHDdpxWAYMUaGoTgPNXYbqZjiPOmLypAz+iO/g==", "9876543021", false, "5c7ba207-a12c-44a8-b1b2-9ba375b5e162", false, "admin@admin.com" },
                    { "f86bf7a1-c0b4-432e-83e4-ccaaf2ebb8ab", 0, "2be0fdf3-aa32-4512-a3fa-501d94891c48", "user3@mail.com", false, "User3", "Userov3", false, null, "USER3@MAIL.COM", "USER3@MAIL.COM", "AQAAAAEAACcQAAAAENO7Sht3f4n/AFCDyIt+johJ+DfX/oPHUKL5jCC7vu/vWTq8fVepjNXLzqIRe6Hp7w==", "132547693", false, "d13d8452-b0b8-4252-9793-1ca15481cfda", false, "user3@mail.com" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "Id", "IsDeleted", "Name", "OwnerId" },
                values: new object[,]
                {
                    { "1dfe1780-daed-4198-8360-378aa33c5411", false, "Bank", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "42e54cf1-dc38-474a-814d-abdd97ec332e", false, "Cash Money", "bcb4f072-ecca-43c9-ab26-c060c6f364e4" },
                    { "cea9346d-bcf4-41aa-aa11-5ddb0b7e6a61", false, "Bank Money", "bcb4f072-ecca-43c9-ab26-c060c6f364e4" },
                    { "daef2351-e2e9-43b9-b908-8d7d00bf3df6", false, "Savings", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "f4c3803a-7ed5-4d78-9038-7b21bf08a040", false, "Cash", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsDeleted", "Name", "OwnerId" },
                values: new object[,]
                {
                    { "081a7be8-15c4-426e-872c-dfaf805e3fec", false, "Salary", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "459dc945-0d2c-4a07-a2aa-55b4c5e57f9f", false, "Dividents", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "93cebd34-a9f5-4862-a8c9-3b6eea63e94c", false, "Food & Drink", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "96e441e3-c5a6-427f-bb32-85940242d9ee", false, "Medical & Healthcare", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "b58a7947-eecf-40d0-b84e-c6947fcbfd86", false, "Transport", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "d59cbb57-3b9e-4b37-9b74-a375eecba8c8", false, "Utilities", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "e03634d5-1970-4e01-8568-42756e9ad973", false, "Money Transfer", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "e241b89f-b094-4f79-bb09-efc6f47c2cb3", false, "Initial Balance", "dea12856-c198-4129-b3f3-b893d8395082" }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "IsDeleted", "Name", "OwnerId" },
                values: new object[,]
                {
                    { "2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f", false, "USD", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "3bf454ad-941b-4ab6-a1ad-c212bfc46e7d", false, "BGN", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { "8370adf0-ef14-465d-8394-215014aaf7c4", false, "GBP", "bcb4f072-ecca-43c9-ab26-c060c6f364e4" },
                    { "c7edb668-a98e-4bc9-800c-fffbe9747d02", false, "SEK", "bcb4f072-ecca-43c9-ab26-c060c6f364e4" },
                    { "dab2761d-acb1-43bc-b56b-0d9c241c8882", false, "EUR", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" }
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
                name: "IX_AccountTypes_OwnerId",
                table: "AccountTypes",
                column: "OwnerId");

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
                name: "IX_Categories_OwnerId",
                table: "Categories",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_OwnerId",
                table: "Currencies",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OwnerId",
                table: "Transactions",
                column: "OwnerId");
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
