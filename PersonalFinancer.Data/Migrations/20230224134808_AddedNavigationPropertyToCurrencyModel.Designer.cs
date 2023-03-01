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
    [Migration("20230224134808_AddedNavigationPropertyToCurrencyModel")]
    partial class AddedNavigationPropertyToCurrencyModel
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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AccountTypeId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            Id = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            AccountTypeId = new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
                            Balance = 200m,
                            CurrencyId = new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
                            IsDeleted = false,
                            Name = "Cash BGN",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            AccountTypeId = new Guid("1dfe1780-daed-4198-8360-378aa33c5411"),
                            Balance = 1834.78m,
                            CurrencyId = new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
                            IsDeleted = false,
                            Name = "Bank BGN",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            AccountTypeId = new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
                            Balance = 600m,
                            CurrencyId = new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
                            IsDeleted = false,
                            Name = "Cash EUR",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            AccountTypeId = new Guid("1dfe1780-daed-4198-8360-378aa33c5411"),
                            Balance = 200m,
                            CurrencyId = new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
                            IsDeleted = false,
                            Name = "Bank EUR",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("303430dc-63a3-4436-8907-a274ec29f608"),
                            AccountTypeId = new Guid("1dfe1780-daed-4198-8360-378aa33c5411"),
                            Balance = 200m,
                            CurrencyId = new Guid("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"),
                            IsDeleted = false,
                            Name = "Bank USD",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.AccountType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AccountTypes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
                            IsDeleted = false,
                            Name = "Cash"
                        },
                        new
                        {
                            Id = new Guid("1dfe1780-daed-4198-8360-378aa33c5411"),
                            IsDeleted = false,
                            Name = "Bank Account"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

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

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

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
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "33259921-b57d-46f0-95dd-93a0eabe2148",
                            Email = "petar@mail.com",
                            EmailConfirmed = false,
                            FirstName = "Petar",
                            LastName = "Petrov",
                            LockoutEnabled = false,
                            NormalizedEmail = "PETAR@MAIL.COM",
                            NormalizedUserName = "PETAR@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEBaXk9U3fqQAaN9lA/0yDQXUZipRj5/GPsGtWAwIjfaNDdD0vRu+CemywdTwCZB/XA==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "7d6240eb-09e6-438c-87d1-c152fa8a6ec3",
                            TwoFactorEnabled = false,
                            UserName = "petar@mail.com"
                        },
                        new
                        {
                            Id = "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "69e4defc-4a1d-4bb3-a084-08e6624cfafb",
                            Email = "teodor@mail.com",
                            EmailConfirmed = false,
                            FirstName = "Teodor",
                            LastName = "Lesly",
                            LockoutEnabled = false,
                            NormalizedEmail = "TEODOR@MAIL.COM",
                            NormalizedUserName = "TEODOR@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEFymq9hycCs3lAXmQxUDMjr4r7RJ3wZBetmref4MYLRLs0T+T269tlujjJ1jx9lDmA==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "f1e2e607-1c37-4d72-a0e2-f533dee6a7bc",
                            TwoFactorEnabled = false,
                            UserName = "teodor@mail.com"
                        },
                        new
                        {
                            Id = "dea12856-c198-4129-b3f3-b893d8395082",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "8590f857-6a75-42fa-959a-647988e3c79e",
                            Email = "admin@admin.com",
                            EmailConfirmed = false,
                            FirstName = "Great",
                            LastName = "Admin",
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@ADMIN.COM",
                            NormalizedUserName = "ADMIN@ADMIN.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAECuU1yJ68Q3JsGdOA9ISqe7Q5BuCZyiwKD16TG2pptl9vNvcNsl9ZLoqMWb/QqTEvQ==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "7680d0fd-7915-4785-a62b-0bf191fe2087",
                            TwoFactorEnabled = false,
                            UserName = "admin@admin.com"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            IsDeleted = false,
                            Name = "Initial Balance"
                        },
                        new
                        {
                            Id = new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"),
                            IsDeleted = false,
                            Name = "Food & Drink"
                        },
                        new
                        {
                            Id = new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"),
                            IsDeleted = false,
                            Name = "Utilities"
                        },
                        new
                        {
                            Id = new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
                            IsDeleted = false,
                            Name = "Transport"
                        },
                        new
                        {
                            Id = new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"),
                            IsDeleted = false,
                            Name = "Medical & Healthcare"
                        },
                        new
                        {
                            Id = new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"),
                            IsDeleted = false,
                            Name = "Salary"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
                            Name = "BGN"
                        },
                        new
                        {
                            Id = new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
                            Name = "EUR"
                        },
                        new
                        {
                            Id = new Guid("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"),
                            Name = "USD"
                        });
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Refference")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Transactions");

                    b.HasData(
                        new
                        {
                            Id = new Guid("c474daee-0e6e-4629-96db-a12376dc07f8"),
                            AccountId = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            Amount = 200m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2531),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("d97d35f0-bc1b-43c4-80ea-792d972ac252"),
                            AccountId = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            Amount = 5.65m,
                            CategoryId = new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"),
                            CreatedOn = new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2544),
                            Refference = "Lunch",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("399bea37-4f28-4bf8-a569-53bd5fc1ec33"),
                            AccountId = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            Amount = 4.80m,
                            CategoryId = new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
                            CreatedOn = new DateTime(2023, 1, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2547),
                            Refference = "Taxi",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("d4010356-2a6d-45bf-ab24-339348487a05"),
                            AccountId = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            Amount = 1834.78m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2564),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("bf2a3f73-9f51-4c39-9825-88bad0042591"),
                            AccountId = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            Amount = 100.00m,
                            CategoryId = new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"),
                            CreatedOn = new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2567),
                            Refference = "Electricity bill",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("94f45fbd-c010-41e4-a5e5-e07e5e5712ee"),
                            AccountId = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            Amount = 1000.00m,
                            CategoryId = new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"),
                            CreatedOn = new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2573),
                            Refference = "Salary",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("5f5de9d7-dadb-40e0-8905-5a0d4df21027"),
                            AccountId = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            Amount = 600m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2576),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("c7cf89f8-1b16-46b7-bd37-43be0c0e1d08"),
                            AccountId = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            Amount = 24.29m,
                            CategoryId = new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"),
                            CreatedOn = new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2579),
                            Refference = "Health Insurance",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("8498f976-c824-49cf-bd9b-aa80a544f0fb"),
                            AccountId = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            Amount = 250m,
                            CategoryId = new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"),
                            CreatedOn = new DateTime(2023, 2, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2583),
                            Refference = "Salary",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("36879b99-5f96-4863-b04e-e303fab4f387"),
                            AccountId = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            Amount = 200m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 11, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2587),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("c28f1fcc-52f2-47f1-bcea-528c9827c89e"),
                            AccountId = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            Amount = 750m,
                            CategoryId = new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"),
                            CreatedOn = new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2590),
                            Refference = "Salary",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("d3406ef6-5d07-46f8-81fb-6a7cdc71f517"),
                            AccountId = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            Amount = 49.99m,
                            CategoryId = new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
                            CreatedOn = new DateTime(2022, 12, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2596),
                            Refference = "Flight ticket",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("997d20b7-f516-4502-8ed4-70d54abd759d"),
                            AccountId = new Guid("303430dc-63a3-4436-8907-a274ec29f608"),
                            Amount = 1487.23m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 6, 24, 13, 48, 7, 449, DateTimeKind.Utc).AddTicks(2599),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        });
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
                        .WithMany()
                        .HasForeignKey("AccountTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountType");

                    b.Navigation("Currency");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.AccountType", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Category", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Currency", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Transaction", b =>
                {
                    b.HasOne("PersonalFinancer.Data.Models.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalFinancer.Data.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("PersonalFinancer.Data.Models.Account", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}