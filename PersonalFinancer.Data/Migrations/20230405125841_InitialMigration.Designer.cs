﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonalFinancer.Data;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    [DbContext(typeof(PersonalFinancerDbContext))]
    [Migration("20230405125841_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CurrencyId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AccountTypeId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.AccountType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("AccountTypes");

                    b.HasData(
                        new
                        {
                            Id = "f4c3803a-7ed5-4d78-9038-7b21bf08a040",
                            IsDeleted = false,
                            Name = "Cash",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "1dfe1780-daed-4198-8360-378aa33c5411",
                            IsDeleted = false,
                            Name = "Bank",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "daef2351-e2e9-43b9-b908-8d7d00bf3df6",
                            IsDeleted = false,
                            Name = "Savings",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "42e54cf1-dc38-474a-814d-abdd97ec332e",
                            IsDeleted = false,
                            Name = "Cash Money",
                            OwnerId = "bcb4f072-ecca-43c9-ab26-c060c6f364e4"
                        },
                        new
                        {
                            Id = "cea9346d-bcf4-41aa-aa11-5ddb0b7e6a61",
                            IsDeleted = false,
                            Name = "Bank Money",
                            OwnerId = "bcb4f072-ecca-43c9-ab26-c060c6f364e4"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                            ConcurrencyStamp = "c116946b-d4b0-451e-aa48-604eb9046dca",
                            Email = "petar@mail.com",
                            FirstName = "Petar",
                            LastName = "Petrov",
                            NormalizedEmail = "PETAR@MAIL.COM",
                            NormalizedUserName = "PETAR@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEDAC2zwlVKOkUjIJGDVqzOmAW2Yt6d0acjKcQ6uMG1wNYLjaYxMoGAe1kJm3PKlqEQ==",
                            PhoneNumber = "1234567890",
                            SecurityStamp = "4bbbd9ef-2ca7-4b7b-93c1-7a3b927faa68",
                            UserName = "petar@mail.com"
                        },
                        new
                        {
                            Id = "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                            ConcurrencyStamp = "36f26458-e197-49fd-843c-64db9425812b",
                            Email = "teodor@mail.com",
                            FirstName = "Teodor",
                            LastName = "Lesly",
                            NormalizedEmail = "TEODOR@MAIL.COM",
                            NormalizedUserName = "TEODOR@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEPtjgzspGhd6+Vhmx04kyoGRtchn4ZQsKEvQsDbVZy31SgFCUPsbP8vd8oHM2pslvw==",
                            PhoneNumber = "1325476980",
                            SecurityStamp = "82c31886-114b-4fe3-b4f7-68aae8d308a7",
                            UserName = "teodor@mail.com"
                        },
                        new
                        {
                            Id = "70425af1-d5b8-4095-abd5-6ecf64be78db",
                            ConcurrencyStamp = "6fd4f499-7df2-4b46-bc5a-8b4831f44d80",
                            Email = "user3@mail.com",
                            FirstName = "User3",
                            LastName = "Userov3",
                            NormalizedEmail = "USER3@MAIL.COM",
                            NormalizedUserName = "USER3@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEENfONr9SoD+XZoYt/lRBSCBDEQUZDXxtw3K/Cv1aYUIqNkT2foMqMAJ9JjvnFbaHA==",
                            PhoneNumber = "132547693",
                            SecurityStamp = "39768d8d-bcf4-4d98-9505-895f7f076766",
                            UserName = "user3@mail.com"
                        },
                        new
                        {
                            Id = "0b73dd4d-440d-4d28-84a2-73d34e1b5a1b",
                            ConcurrencyStamp = "63cd5274-ca36-4366-be63-96f5fc6c227f",
                            Email = "user4@mail.com",
                            FirstName = "User4",
                            LastName = "Userov4",
                            NormalizedEmail = "USER4@MAIL.COM",
                            NormalizedUserName = "USER4@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEI/3VqoJ3aQV+/tEEZPzVpqCfC+op5agaSL7aDk6B6jeeQMcFkNxlBkZO1gNBdc5Kg==",
                            PhoneNumber = "132547694",
                            SecurityStamp = "a7349fee-24f8-4505-ac85-0c4c833c28b7",
                            UserName = "user4@mail.com"
                        },
                        new
                        {
                            Id = "9c1ed10d-6b74-4751-88b3-d78f83a9925e",
                            ConcurrencyStamp = "83eb971d-427e-4ce3-a4a5-ae063dd4eb15",
                            Email = "user5@mail.com",
                            FirstName = "User5",
                            LastName = "Userov5",
                            NormalizedEmail = "USER5@MAIL.COM",
                            NormalizedUserName = "USER5@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEFuw2ESuBvy/7fECFNPIyjq39fRigL00fCSe/C4e8kx6bD6M3vwFZr68qSClwWwP0w==",
                            PhoneNumber = "132547695",
                            SecurityStamp = "9f165a66-e1a0-45cf-83cd-c11f1c6fd5f8",
                            UserName = "user5@mail.com"
                        },
                        new
                        {
                            Id = "47f33c7f-d0c2-4d85-b50a-f9f0fc2689ef",
                            ConcurrencyStamp = "75a93bff-c3c9-4ba4-bd80-27b76476704c",
                            Email = "user6@mail.com",
                            FirstName = "User6",
                            LastName = "Userov6",
                            NormalizedEmail = "USER6@MAIL.COM",
                            NormalizedUserName = "USER6@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEEzDKBdjMOo+skTb3Oor3WAfiMpql6zk4+Oar/sgR6/5osrYYJDLxDmPE5/a6ng2Hg==",
                            PhoneNumber = "132547696",
                            SecurityStamp = "72f48e1a-362f-4446-af9e-0e84ae364a03",
                            UserName = "user6@mail.com"
                        },
                        new
                        {
                            Id = "b1c3d5b1-21d6-41b8-8706-fe480ac261b3",
                            ConcurrencyStamp = "34f56131-5534-4631-8800-f12b37d9870a",
                            Email = "user7@mail.com",
                            FirstName = "User7",
                            LastName = "Userov7",
                            NormalizedEmail = "USER7@MAIL.COM",
                            NormalizedUserName = "USER7@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEEUQtfTaUb1l3Q0o1XcpghhAa41d2y/7UZ1wf79HxOiVf7TpWeSQFPqeaiZ/m+2tgA==",
                            PhoneNumber = "132547697",
                            SecurityStamp = "bb988a90-2dba-4381-925c-45dcc200776e",
                            UserName = "user7@mail.com"
                        },
                        new
                        {
                            Id = "3edfe677-4c7e-4bd7-b8d4-b9a867b6659b",
                            ConcurrencyStamp = "32d0bb16-0f77-4a0e-ba67-3d0c11e0b2d6",
                            Email = "user8@mail.com",
                            FirstName = "User8",
                            LastName = "Userov8",
                            NormalizedEmail = "USER8@MAIL.COM",
                            NormalizedUserName = "USER8@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEB8ijerHvaamnaCpNujD59d/goRlqKCYsHCZFjWMwmVM9iyLraIkSCYSIfjaK8b7BQ==",
                            PhoneNumber = "132547698",
                            SecurityStamp = "c1f4fc77-665f-426f-962f-c32b812a7e59",
                            UserName = "user8@mail.com"
                        },
                        new
                        {
                            Id = "76d9469c-5b51-42d8-8aad-9825c49cf05e",
                            ConcurrencyStamp = "92593e0a-2979-4bd5-b65f-b3b006b0200b",
                            Email = "user9@mail.com",
                            FirstName = "User9",
                            LastName = "Userov9",
                            NormalizedEmail = "USER9@MAIL.COM",
                            NormalizedUserName = "USER9@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEMa5BfPHZNmGGd332XDG5oRa/HhpgSkq1HceRJprLE2oFGeXvMHz86ohak0Cdj1mbA==",
                            PhoneNumber = "132547699",
                            SecurityStamp = "6079a6c5-7844-45d0-8cd2-4fd3bb724aff",
                            UserName = "user9@mail.com"
                        },
                        new
                        {
                            Id = "1701fe87-c6fd-4e5c-8ac7-514dbea7d1a2",
                            ConcurrencyStamp = "6517d258-de8c-43c6-b7e9-34d0c176743d",
                            Email = "user10@mail.com",
                            FirstName = "User10",
                            LastName = "Userov10",
                            NormalizedEmail = "USER10@MAIL.COM",
                            NormalizedUserName = "USER10@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAELsv3wqGyUo/DnHrBhchH+qNUj1lDuSlYBFCVz4/GCJK2be6c4XhaMRdu+/GI1u8BQ==",
                            PhoneNumber = "1325476910",
                            SecurityStamp = "27c8e8ea-a52c-475c-b11e-1a46e023fed4",
                            UserName = "user10@mail.com"
                        },
                        new
                        {
                            Id = "57662463-4aaf-4366-8676-5b900853b1b5",
                            ConcurrencyStamp = "46707765-b311-4513-bf74-1030de573256",
                            Email = "user11@mail.com",
                            FirstName = "User11",
                            LastName = "Userov11",
                            NormalizedEmail = "USER11@MAIL.COM",
                            NormalizedUserName = "USER11@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEKnFtcMDtkp3ojnwzvA7ax8wFUz+2+8p0h6VmjqmL0a2ZQov0YKru1B1Ku8wtOY0yw==",
                            PhoneNumber = "1325476911",
                            SecurityStamp = "6a53c96e-46f3-4db0-baa4-c225e903e6e2",
                            UserName = "user11@mail.com"
                        },
                        new
                        {
                            Id = "1cc73160-5f3d-4c41-b351-430168e2d12e",
                            ConcurrencyStamp = "b2394a9c-b2a7-4607-9b0b-48e3bbee0a44",
                            Email = "user12@mail.com",
                            FirstName = "User12",
                            LastName = "Userov12",
                            NormalizedEmail = "USER12@MAIL.COM",
                            NormalizedUserName = "USER12@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEBtscTMcbI50f3922SDDsR+PDjqUi2Qk+GkxUOrD81Ag6pPRiCDUU1zzQZoPgPXdHg==",
                            PhoneNumber = "1325476912",
                            SecurityStamp = "2567fda8-c6ed-4f15-bd42-250690ea4ba9",
                            UserName = "user12@mail.com"
                        },
                        new
                        {
                            Id = "06df8f30-07c1-45cc-a5c7-214dac6155a1",
                            ConcurrencyStamp = "a44e75fc-0166-4e76-b591-5f259e096511",
                            Email = "user13@mail.com",
                            FirstName = "User13",
                            LastName = "Userov13",
                            NormalizedEmail = "USER13@MAIL.COM",
                            NormalizedUserName = "USER13@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEH3aoQ3xo6ufxRZzveCHOZ3+ooUUA2JkockRzdGZBykZdrl1LaDnPtsnABPR4DP5Jw==",
                            PhoneNumber = "1325476913",
                            SecurityStamp = "66440d01-e5c4-4af1-9a16-a38afd4eafed",
                            UserName = "user13@mail.com"
                        },
                        new
                        {
                            Id = "dea12856-c198-4129-b3f3-b893d8395082",
                            ConcurrencyStamp = "9f808318-9e8a-4c16-aaa0-b46956149963",
                            Email = "admin@admin.com",
                            FirstName = "Great",
                            LastName = "Admin",
                            NormalizedEmail = "ADMIN@ADMIN.COM",
                            NormalizedUserName = "ADMIN@ADMIN.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEB75v3qlloDcq71my5XtONhPWUV3dIa3p8LhQq2y4jB7zqdlxVl+Ek5JP0Lkg7rXwg==",
                            PhoneNumber = "9876543021",
                            SecurityStamp = "61f0bf2b-2c2f-4294-aed4-3f5227e97209",
                            UserName = "admin@admin.com"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = "e241b89f-b094-4f79-bb09-efc6f47c2cb3",
                            IsDeleted = false,
                            Name = "Initial Balance",
                            OwnerId = "dea12856-c198-4129-b3f3-b893d8395082"
                        },
                        new
                        {
                            Id = "93cebd34-a9f5-4862-a8c9-3b6eea63e94c",
                            IsDeleted = false,
                            Name = "Food & Drink",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "d59cbb57-3b9e-4b37-9b74-a375eecba8c8",
                            IsDeleted = false,
                            Name = "Utilities",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "b58a7947-eecf-40d0-b84e-c6947fcbfd86",
                            IsDeleted = false,
                            Name = "Transport",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "96e441e3-c5a6-427f-bb32-85940242d9ee",
                            IsDeleted = false,
                            Name = "Medical & Healthcare",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "081a7be8-15c4-426e-872c-dfaf805e3fec",
                            IsDeleted = false,
                            Name = "Salary",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "e03634d5-1970-4e01-8568-42756e9ad973",
                            IsDeleted = false,
                            Name = "Money Transfer",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "459dc945-0d2c-4a07-a2aa-55b4c5e57f9f",
                            IsDeleted = false,
                            Name = "Dividents",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Currency", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = "3bf454ad-941b-4ab6-a1ad-c212bfc46e7d",
                            IsDeleted = false,
                            Name = "BGN",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "dab2761d-acb1-43bc-b56b-0d9c241c8882",
                            IsDeleted = false,
                            Name = "EUR",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f",
                            IsDeleted = false,
                            Name = "USD",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = "c7edb668-a98e-4bc9-800c-fffbe9747d02",
                            IsDeleted = false,
                            Name = "SEK",
                            OwnerId = "bcb4f072-ecca-43c9-ab26-c060c6f364e4"
                        },
                        new
                        {
                            Id = "8370adf0-ef14-465d-8394-215014aaf7c4",
                            IsDeleted = false,
                            Name = "GBP",
                            OwnerId = "bcb4f072-ecca-43c9-ab26-c060c6f364e4"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsInitialBalance")
                        .HasColumnType("bit");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Refference")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Account", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.AccountType", "AccountType")
                        .WithMany("Accounts")
                        .HasForeignKey("AccountTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.Currency", "Currency")
                        .WithMany("Accounts")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "Owner")
                        .WithMany("Accounts")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountType");

                    b.Navigation("Currency");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.AccountType", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "Owner")
                        .WithMany("AccountTypes")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Category", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "Owner")
                        .WithMany("Categories")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Currency", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "Owner")
                        .WithMany("Currencies")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Transaction", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.Category", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "Owner")
                        .WithMany("Transactions")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Category");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Account", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.AccountType", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.ApplicationUser", b =>
                {
                    b.Navigation("AccountTypes");

                    b.Navigation("Accounts");

                    b.Navigation("Categories");

                    b.Navigation("Currencies");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Category", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Currency", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}