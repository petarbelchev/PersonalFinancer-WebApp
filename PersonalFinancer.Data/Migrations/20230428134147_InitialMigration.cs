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
                    { "0314e891-80e5-4dd3-b0ba-3416f8397ce8", 0, "d0c05c57-ef5f-489d-a9c0-af2d6bac21d0", "user11@mail.com", true, "User11", "User11", false, null, "USER11@MAIL.COM", "USER11@MAIL.COM", "AQAAAAEAACcQAAAAEGwROIaHp/lebC5C0BENaUw8ZDoLjtld61OG7QlE7Qtacs89Vm9tezNr96quLiP8aQ==", "0000000000", false, "ec1425cc-ab90-4c53-a762-56a193e9fb05", false, "user11@mail.com" },
                    { "3f9c0798-5ef7-4512-81f3-8de9f815e762", 0, "07529379-7165-4691-856e-b33dadecc1c6", "user6@mail.com", true, "User6", "User6", false, null, "USER6@MAIL.COM", "USER6@MAIL.COM", "AQAAAAEAACcQAAAAEJf0mjW20Id+zgqiMgT3OrZKmWs/Nn7l/kC26TV3MrpduXWQ/liMIkvXhyOAIU0ILA==", "0000000000", false, "9875800f-c4dd-454c-b73f-9fbaa442f27f", false, "user6@mail.com" },
                    { "4ac29d46-7b8f-485d-b0d7-b49179f8a8f1", 0, "cc8d3d5e-4de4-4679-819c-2b844785bb39", "user9@mail.com", true, "User9", "User9", false, null, "USER9@MAIL.COM", "USER9@MAIL.COM", "AQAAAAEAACcQAAAAEPvy6tM/2cwRfDzsNmuqENPRMqXvfFuVaz5Y9VvjzuzJ7pJNhFb2scLXSQiaPeI9mg==", "0000000000", false, "0dca498f-2737-475f-a6d9-c5dc17950c21", false, "user9@mail.com" },
                    { "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e", 0, "b12a35c7-dcec-40e0-b8aa-f0d5976f50a5", "petar@mail.com", true, "Petar", "Petrov", false, null, "PETAR@MAIL.COM", "PETAR@MAIL.COM", "AQAAAAEAACcQAAAAEJVaHTFHlCHicZjaaEuOggMSz69lLD0g7C0dRhi9S30xCO18WMRS0WEPM/Un2Au/SQ==", "1234567890", false, "20796eb1-107c-4e41-b67f-30404af5b616", false, "petar@mail.com" },
                    { "7abc426f-0545-4c45-bb62-ce6332f0ccb0", 0, "b78d2b38-5e71-4d3d-a5ca-cc90823fb668", "user3@mail.com", true, "User3", "User3", false, null, "USER3@MAIL.COM", "USER3@MAIL.COM", "AQAAAAEAACcQAAAAEOFzA92DjwAsX84EYmKQhNNkAVbKkVIUpC1Uy2OzsJNxUtPVG8IdjC9WCCbeBwYMOg==", "0000000000", false, "20a15185-ef58-4de6-9af2-af6023bdd25c", false, "user3@mail.com" },
                    { "8cd62f7a-bd8a-46e3-9caf-f0b4cf1f0c26", 0, "cb8e733a-8f8a-448a-946a-ae5d0beeab86", "user8@mail.com", true, "User8", "User8", false, null, "USER8@MAIL.COM", "USER8@MAIL.COM", "AQAAAAEAACcQAAAAEDWpk2lylWaEDCaQg2PqzzT6yYmYlgxR9rF6/Eva8DYuWMSBEWdWIqNAaR0laLh6BQ==", "0000000000", false, "b98c05f4-d9fa-425a-bc11-8165fa4d8e16", false, "user8@mail.com" },
                    { "a94e70ce-b628-46e3-9c12-150147341345", 0, "36f920fb-8a39-4c30-a293-be783b2b07d9", "user10@mail.com", true, "User10", "User10", false, null, "USER10@MAIL.COM", "USER10@MAIL.COM", "AQAAAAEAACcQAAAAENyDOLvcgL4jReEJZ398gM4TN6c2Tu1MfVC3oIaNo+fiYoTNgSAM4eewh8BjI4fEdQ==", "0000000000", false, "95947cb8-e538-46d6-98e6-5db318b8f63d", false, "user10@mail.com" },
                    { "b636ef12-242e-49ec-8f3f-7ba5c39da705", 0, "55187ebf-ec94-4f67-8e84-8c7374db12d4", "user7@mail.com", true, "User7", "User7", false, null, "USER7@MAIL.COM", "USER7@MAIL.COM", "AQAAAAEAACcQAAAAECmODnnlQwX8NYvx2kwaNRhbZaUxSdNAwv5W9Y7LzyrkEYqKCUKp3dvA6GEfPBzGvQ==", "0000000000", false, "6afa6219-dd85-48a4-bedf-bfb87aadb4a7", false, "user7@mail.com" },
                    { "d4cd6b5a-59ae-4878-bbd3-e07a85229b8a", 0, "bdeb8c87-39fb-4364-acdd-0c4ff0322cf7", "user4@mail.com", true, "User4", "User4", false, null, "USER4@MAIL.COM", "USER4@MAIL.COM", "AQAAAAEAACcQAAAAEAmIWv1Zu/FfPfj284yuOQoc7/52UckT+kgB6JGlCWbYbP1JtYjvU3oZA0WDT6sL9A==", "0000000000", false, "7855fcdb-761e-4fc9-873b-dcc1804e8063", false, "user4@mail.com" },
                    { "dea12856-c198-4129-b3f3-b893d8395082", 0, "8a929e15-4134-4b0b-90cb-f425df31fa8f", "admin@admin.com", true, "Great", "Admin", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEHHfj8i4eLWmgRQhmmMR+f+ScUn+DJxYW7YVGHvx+BqiQD6dhg2jyVhHnlIk9Gud1g==", "9876543021", false, "1dffe681-b79c-4969-8a91-5e674da629d7", false, "admin@admin.com" },
                    { "ea00df27-92c3-4a09-adb8-a6a97bb29487", 0, "d3d0c752-fb86-4636-bcae-8e76301e259d", "user12@mail.com", true, "User12", "User12", false, null, "USER12@MAIL.COM", "USER12@MAIL.COM", "AQAAAAEAACcQAAAAEAu7aV39nBwLHzHnYgG4neZ/kTF6siVvWxgoGwAevMgAWHR3aAtq57CXxoEb96xKeQ==", "0000000000", false, "df7b1424-1a15-4030-8dbd-d3a9bf91c963", false, "user12@mail.com" },
                    { "f5f2dfd4-f46c-483a-b4c6-56482b4c0491", 0, "a4c973cf-12a1-4816-a3ec-92ef9fe0b037", "user13@mail.com", true, "User13", "User13", false, null, "USER13@MAIL.COM", "USER13@MAIL.COM", "AQAAAAEAACcQAAAAEN2VLaIIkdwo8LX/kzNcqqhUiDruJt52NICYLRh3hGsdHQBcJaAbpmT5YOIahklwig==", "0000000000", false, "68ad99d0-320c-44ff-b553-bce9ada3860a", false, "user13@mail.com" },
                    { "fd79173d-0dbd-4d1b-b628-bf96c1a4c93d", 0, "a0140fda-24c5-4b9f-bfc1-7c894b47ff39", "user5@mail.com", true, "User5", "User5", false, null, "USER5@MAIL.COM", "USER5@MAIL.COM", "AQAAAAEAACcQAAAAEFOgjgGhFIITusqfdcd31ZgBH4Cc5ieMo9SoMhc2CgI+b9HL+UEsmwMwdBk9FSf2vw==", "0000000000", false, "76bda890-7a3c-4708-aea2-c7789d377798", false, "user5@mail.com" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "Id", "IsDeleted", "Name", "OwnerId" },
                values: new object[,]
                {
                    { "1dfe1780-daed-4198-8360-378aa33c5411", false, "Bank", "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
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
