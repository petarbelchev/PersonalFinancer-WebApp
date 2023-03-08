﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonalFinancer.Data;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    [DbContext(typeof(PersonalFinancerDbContext))]
    partial class PersonalFinancerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uniqueidentifier");

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

                    b.HasData(
                        new
                        {
                            Id = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            AccountTypeId = new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
                            Balance = 189.55m,
                            CurrencyId = new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
                            IsDeleted = false,
                            Name = "Cash BGN",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            AccountTypeId = new Guid("1dfe1780-daed-4198-8360-378aa33c5411"),
                            Balance = 2734.78m,
                            CurrencyId = new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
                            IsDeleted = false,
                            Name = "Bank BGN",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            AccountTypeId = new Guid("f4c3803a-7ed5-4d78-9038-7b21bf08a040"),
                            Balance = 825.71m,
                            CurrencyId = new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
                            IsDeleted = false,
                            Name = "Cash EUR",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            AccountTypeId = new Guid("1dfe1780-daed-4198-8360-378aa33c5411"),
                            Balance = 900.01m,
                            CurrencyId = new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
                            IsDeleted = false,
                            Name = "Bank EUR",
                            OwnerId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e"
                        },
                        new
                        {
                            Id = new Guid("303430dc-63a3-4436-8907-a274ec29f608"),
                            AccountTypeId = new Guid("1dfe1780-daed-4198-8360-378aa33c5411"),
                            Balance = 1487.23m,
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
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

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
                            ConcurrencyStamp = "e1909cba-6a0e-4956-a5ed-961608230294",
                            Email = "petar@mail.com",
                            EmailConfirmed = false,
                            FirstName = "Petar",
                            LastName = "Petrov",
                            LockoutEnabled = false,
                            NormalizedEmail = "PETAR@MAIL.COM",
                            NormalizedUserName = "PETAR@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEG99NnwggvkIPSDY2wBYtMT+t8/duvhPkAdZFfXGRxxCNYuOp2P3FKbScDtQ6PUyGg==",
                            PhoneNumber = "1234567890",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "593c7b8c-8d60-4a78-a24e-932670de17bd",
                            TwoFactorEnabled = false,
                            UserName = "petar@mail.com"
                        },
                        new
                        {
                            Id = "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "3e503f98-b332-4998-aeef-2e97d017473f",
                            Email = "teodor@mail.com",
                            EmailConfirmed = false,
                            FirstName = "Teodor",
                            LastName = "Lesly",
                            LockoutEnabled = false,
                            NormalizedEmail = "TEODOR@MAIL.COM",
                            NormalizedUserName = "TEODOR@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEIb4Isv5P5X7s9YPqW+iHWhNG/K/ovLIjbofSv6KZIeYGxaW8XpTyKsDjXQkfblnXA==",
                            PhoneNumber = "1325476980",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "98d90ce9-de1a-426b-9f9d-b34bbf16df0c",
                            TwoFactorEnabled = false,
                            UserName = "teodor@mail.com"
                        },
                        new
                        {
                            Id = "dea12856-c198-4129-b3f3-b893d8395082",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "bde8d3fd-30de-4807-92aa-4abbb087fbd3",
                            Email = "admin@admin.com",
                            EmailConfirmed = false,
                            FirstName = "Great",
                            LastName = "Admin",
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@ADMIN.COM",
                            NormalizedUserName = "ADMIN@ADMIN.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEMVIK90e48owRodrR9m89jaR2BkqxnorXJKYUIpDi52MYiUDk/XXoSVqe7YzMUS+sQ==",
                            PhoneNumber = "9876543021",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "bae940df-11d0-4a94-8102-27c5cf37cd42",
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
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

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

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3bf454ad-941b-4ab6-a1ad-c212bfc46e7d"),
                            IsDeleted = false,
                            Name = "BGN"
                        },
                        new
                        {
                            Id = new Guid("dab2761d-acb1-43bc-b56b-0d9c241c8882"),
                            IsDeleted = false,
                            Name = "EUR"
                        },
                        new
                        {
                            Id = new Guid("2f2c29e5-4463-4d5d-bfd2-e0f973c24e8f"),
                            IsDeleted = false,
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
                        .HasPrecision(18, 2)
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
                            Id = new Guid("7661ddfb-257c-4c3a-a02f-89f6541a0c06"),
                            AccountId = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            Amount = 200m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(5463),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("2d11697d-b0b9-4b7e-a285-e6968d2415ef"),
                            AccountId = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            Amount = 5.65m,
                            CategoryId = new Guid("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"),
                            CreatedOn = new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6956),
                            Refference = "Lunch",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("1f1ea777-03e7-4176-8b65-69f3cb3c779f"),
                            AccountId = new Guid("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
                            Amount = 4.80m,
                            CategoryId = new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
                            CreatedOn = new DateTime(2023, 2, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6975),
                            Refference = "Taxi",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("481e697e-4b27-4163-92a8-c3d48d943898"),
                            AccountId = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            Amount = 1834.78m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6980),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("ebaa4859-647a-44c5-9630-b9379534cf4b"),
                            AccountId = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            Amount = 100.00m,
                            CategoryId = new Guid("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"),
                            CreatedOn = new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6986),
                            Refference = "Electricity bill",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("2ce5f0dd-1bbe-4a44-b12e-485e5a0d7209"),
                            AccountId = new Guid("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
                            Amount = 1000.00m,
                            CategoryId = new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"),
                            CreatedOn = new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(6999),
                            Refference = "Salary",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("1a8faa14-e3e8-4f06-be52-e92c934380d1"),
                            AccountId = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            Amount = 600m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7004),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("e9a40bde-668c-44cb-a281-1c990299ed5c"),
                            AccountId = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            Amount = 24.29m,
                            CategoryId = new Guid("96e441e3-c5a6-427f-bb32-85940242d9ee"),
                            CreatedOn = new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7027),
                            Refference = "Health Insurance",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("96152eaa-c751-417a-880a-8cbea3815fd9"),
                            AccountId = new Guid("70169197-5c32-4430-ab39-34c776533376"),
                            Amount = 250m,
                            CategoryId = new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"),
                            CreatedOn = new DateTime(2023, 3, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7032),
                            Refference = "Salary",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("56e72652-cb1e-4c80-ba61-74be466a49d6"),
                            AccountId = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            Amount = 200m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 12, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7039),
                            Refference = "Initial Balance",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("14895d28-597f-452a-adc6-a03cb817bd30"),
                            AccountId = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            Amount = 750m,
                            CategoryId = new Guid("081a7be8-15c4-426e-872c-dfaf805e3fec"),
                            CreatedOn = new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7043),
                            Refference = "Salary",
                            TransactionType = 0
                        },
                        new
                        {
                            Id = new Guid("d243b590-117b-4b04-a831-4d983dfc4302"),
                            AccountId = new Guid("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
                            Amount = 49.99m,
                            CategoryId = new Guid("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
                            CreatedOn = new DateTime(2023, 1, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7047),
                            Refference = "Flight ticket",
                            TransactionType = 1
                        },
                        new
                        {
                            Id = new Guid("3ee36d75-1c8b-4945-9836-73f90bcf5be7"),
                            AccountId = new Guid("303430dc-63a3-4436-8907-a274ec29f608"),
                            Amount = 1487.23m,
                            CategoryId = new Guid("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
                            CreatedOn = new DateTime(2022, 7, 8, 11, 40, 6, 78, DateTimeKind.Utc).AddTicks(7053),
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

            modelBuilder.Entity("PersonalFinancer.Data.Models.ApplicationUser", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
