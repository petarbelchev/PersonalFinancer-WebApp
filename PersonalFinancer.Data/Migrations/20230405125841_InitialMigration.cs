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
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                columns: new[] { "Id", "ConcurrencyStamp", "Email", "FirstName", "LastName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "SecurityStamp", "UserName" },
                values: new object[,]
                {
                    { "06df8f30-07c1-45cc-a5c7-214dac6155a1", "a44e75fc-0166-4e76-b591-5f259e096511", "user13@mail.com", "User13", "Userov13", "USER13@MAIL.COM", "USER13@MAIL.COM", "AQAAAAEAACcQAAAAEH3aoQ3xo6ufxRZzveCHOZ3+ooUUA2JkockRzdGZBykZdrl1LaDnPtsnABPR4DP5Jw==", "1325476913", "66440d01-e5c4-4af1-9a16-a38afd4eafed", "user13@mail.com" },
                    { "0b73dd4d-440d-4d28-84a2-73d34e1b5a1b", "63cd5274-ca36-4366-be63-96f5fc6c227f", "user4@mail.com", "User4", "Userov4", "USER4@MAIL.COM", "USER4@MAIL.COM", "AQAAAAEAACcQAAAAEI/3VqoJ3aQV+/tEEZPzVpqCfC+op5agaSL7aDk6B6jeeQMcFkNxlBkZO1gNBdc5Kg==", "132547694", "a7349fee-24f8-4505-ac85-0c4c833c28b7", "user4@mail.com" },
                    { "1701fe87-c6fd-4e5c-8ac7-514dbea7d1a2", "6517d258-de8c-43c6-b7e9-34d0c176743d", "user10@mail.com", "User10", "Userov10", "USER10@MAIL.COM", "USER10@MAIL.COM", "AQAAAAEAACcQAAAAELsv3wqGyUo/DnHrBhchH+qNUj1lDuSlYBFCVz4/GCJK2be6c4XhaMRdu+/GI1u8BQ==", "1325476910", "27c8e8ea-a52c-475c-b11e-1a46e023fed4", "user10@mail.com" },
                    { "1cc73160-5f3d-4c41-b351-430168e2d12e", "b2394a9c-b2a7-4607-9b0b-48e3bbee0a44", "user12@mail.com", "User12", "Userov12", "USER12@MAIL.COM", "USER12@MAIL.COM", "AQAAAAEAACcQAAAAEBtscTMcbI50f3922SDDsR+PDjqUi2Qk+GkxUOrD81Ag6pPRiCDUU1zzQZoPgPXdHg==", "1325476912", "2567fda8-c6ed-4f15-bd42-250690ea4ba9", "user12@mail.com" },
                    { "3edfe677-4c7e-4bd7-b8d4-b9a867b6659b", "32d0bb16-0f77-4a0e-ba67-3d0c11e0b2d6", "user8@mail.com", "User8", "Userov8", "USER8@MAIL.COM", "USER8@MAIL.COM", "AQAAAAEAACcQAAAAEB8ijerHvaamnaCpNujD59d/goRlqKCYsHCZFjWMwmVM9iyLraIkSCYSIfjaK8b7BQ==", "132547698", "c1f4fc77-665f-426f-962f-c32b812a7e59", "user8@mail.com" },
                    { "47f33c7f-d0c2-4d85-b50a-f9f0fc2689ef", "75a93bff-c3c9-4ba4-bd80-27b76476704c", "user6@mail.com", "User6", "Userov6", "USER6@MAIL.COM", "USER6@MAIL.COM", "AQAAAAEAACcQAAAAEEzDKBdjMOo+skTb3Oor3WAfiMpql6zk4+Oar/sgR6/5osrYYJDLxDmPE5/a6ng2Hg==", "132547696", "72f48e1a-362f-4446-af9e-0e84ae364a03", "user6@mail.com" },
                    { "57662463-4aaf-4366-8676-5b900853b1b5", "46707765-b311-4513-bf74-1030de573256", "user11@mail.com", "User11", "Userov11", "USER11@MAIL.COM", "USER11@MAIL.COM", "AQAAAAEAACcQAAAAEKnFtcMDtkp3ojnwzvA7ax8wFUz+2+8p0h6VmjqmL0a2ZQov0YKru1B1Ku8wtOY0yw==", "1325476911", "6a53c96e-46f3-4db0-baa4-c225e903e6e2", "user11@mail.com" },
                    { "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e", "c116946b-d4b0-451e-aa48-604eb9046dca", "petar@mail.com", "Petar", "Petrov", "PETAR@MAIL.COM", "PETAR@MAIL.COM", "AQAAAAEAACcQAAAAEDAC2zwlVKOkUjIJGDVqzOmAW2Yt6d0acjKcQ6uMG1wNYLjaYxMoGAe1kJm3PKlqEQ==", "1234567890", "4bbbd9ef-2ca7-4b7b-93c1-7a3b927faa68", "petar@mail.com" },
                    { "70425af1-d5b8-4095-abd5-6ecf64be78db", "6fd4f499-7df2-4b46-bc5a-8b4831f44d80", "user3@mail.com", "User3", "Userov3", "USER3@MAIL.COM", "USER3@MAIL.COM", "AQAAAAEAACcQAAAAEENfONr9SoD+XZoYt/lRBSCBDEQUZDXxtw3K/Cv1aYUIqNkT2foMqMAJ9JjvnFbaHA==", "132547693", "39768d8d-bcf4-4d98-9505-895f7f076766", "user3@mail.com" },
                    { "76d9469c-5b51-42d8-8aad-9825c49cf05e", "92593e0a-2979-4bd5-b65f-b3b006b0200b", "user9@mail.com", "User9", "Userov9", "USER9@MAIL.COM", "USER9@MAIL.COM", "AQAAAAEAACcQAAAAEMa5BfPHZNmGGd332XDG5oRa/HhpgSkq1HceRJprLE2oFGeXvMHz86ohak0Cdj1mbA==", "132547699", "6079a6c5-7844-45d0-8cd2-4fd3bb724aff", "user9@mail.com" },
                    { "9c1ed10d-6b74-4751-88b3-d78f83a9925e", "83eb971d-427e-4ce3-a4a5-ae063dd4eb15", "user5@mail.com", "User5", "Userov5", "USER5@MAIL.COM", "USER5@MAIL.COM", "AQAAAAEAACcQAAAAEFuw2ESuBvy/7fECFNPIyjq39fRigL00fCSe/C4e8kx6bD6M3vwFZr68qSClwWwP0w==", "132547695", "9f165a66-e1a0-45cf-83cd-c11f1c6fd5f8", "user5@mail.com" },
                    { "b1c3d5b1-21d6-41b8-8706-fe480ac261b3", "34f56131-5534-4631-8800-f12b37d9870a", "user7@mail.com", "User7", "Userov7", "USER7@MAIL.COM", "USER7@MAIL.COM", "AQAAAAEAACcQAAAAEEUQtfTaUb1l3Q0o1XcpghhAa41d2y/7UZ1wf79HxOiVf7TpWeSQFPqeaiZ/m+2tgA==", "132547697", "bb988a90-2dba-4381-925c-45dcc200776e", "user7@mail.com" },
                    { "bcb4f072-ecca-43c9-ab26-c060c6f364e4", "36f26458-e197-49fd-843c-64db9425812b", "teodor@mail.com", "Teodor", "Lesly", "TEODOR@MAIL.COM", "TEODOR@MAIL.COM", "AQAAAAEAACcQAAAAEPtjgzspGhd6+Vhmx04kyoGRtchn4ZQsKEvQsDbVZy31SgFCUPsbP8vd8oHM2pslvw==", "1325476980", "82c31886-114b-4fe3-b4f7-68aae8d308a7", "teodor@mail.com" },
                    { "dea12856-c198-4129-b3f3-b893d8395082", "9f808318-9e8a-4c16-aaa0-b46956149963", "admin@admin.com", "Great", "Admin", "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEB75v3qlloDcq71my5XtONhPWUV3dIa3p8LhQq2y4jB7zqdlxVl+Ek5JP0Lkg7rXwg==", "9876543021", "61f0bf2b-2c2f-4294-aed4-3f5227e97209", "admin@admin.com" }
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
