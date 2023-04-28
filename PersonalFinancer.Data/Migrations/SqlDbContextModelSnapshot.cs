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
    [DbContext(typeof(SqlDbContext))]
    partial class SqlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
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
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

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
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

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
                            ConcurrencyStamp = "b12a35c7-dcec-40e0-b8aa-f0d5976f50a5",
                            Email = "petar@mail.com",
                            EmailConfirmed = true,
                            FirstName = "Petar",
                            LastName = "Petrov",
                            LockoutEnabled = false,
                            NormalizedEmail = "PETAR@MAIL.COM",
                            NormalizedUserName = "PETAR@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEJVaHTFHlCHicZjaaEuOggMSz69lLD0g7C0dRhi9S30xCO18WMRS0WEPM/Un2Au/SQ==",
                            PhoneNumber = "1234567890",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "20796eb1-107c-4e41-b67f-30404af5b616",
                            TwoFactorEnabled = false,
                            UserName = "petar@mail.com"
                        },
                        new
                        {
                            Id = "7abc426f-0545-4c45-bb62-ce6332f0ccb0",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "b78d2b38-5e71-4d3d-a5ca-cc90823fb668",
                            Email = "user3@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User3",
                            LastName = "User3",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER3@MAIL.COM",
                            NormalizedUserName = "USER3@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEOFzA92DjwAsX84EYmKQhNNkAVbKkVIUpC1Uy2OzsJNxUtPVG8IdjC9WCCbeBwYMOg==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "20a15185-ef58-4de6-9af2-af6023bdd25c",
                            TwoFactorEnabled = false,
                            UserName = "user3@mail.com"
                        },
                        new
                        {
                            Id = "d4cd6b5a-59ae-4878-bbd3-e07a85229b8a",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "bdeb8c87-39fb-4364-acdd-0c4ff0322cf7",
                            Email = "user4@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User4",
                            LastName = "User4",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER4@MAIL.COM",
                            NormalizedUserName = "USER4@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEAmIWv1Zu/FfPfj284yuOQoc7/52UckT+kgB6JGlCWbYbP1JtYjvU3oZA0WDT6sL9A==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "7855fcdb-761e-4fc9-873b-dcc1804e8063",
                            TwoFactorEnabled = false,
                            UserName = "user4@mail.com"
                        },
                        new
                        {
                            Id = "fd79173d-0dbd-4d1b-b628-bf96c1a4c93d",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "a0140fda-24c5-4b9f-bfc1-7c894b47ff39",
                            Email = "user5@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User5",
                            LastName = "User5",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER5@MAIL.COM",
                            NormalizedUserName = "USER5@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEFOgjgGhFIITusqfdcd31ZgBH4Cc5ieMo9SoMhc2CgI+b9HL+UEsmwMwdBk9FSf2vw==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "76bda890-7a3c-4708-aea2-c7789d377798",
                            TwoFactorEnabled = false,
                            UserName = "user5@mail.com"
                        },
                        new
                        {
                            Id = "3f9c0798-5ef7-4512-81f3-8de9f815e762",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "07529379-7165-4691-856e-b33dadecc1c6",
                            Email = "user6@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User6",
                            LastName = "User6",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER6@MAIL.COM",
                            NormalizedUserName = "USER6@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEJf0mjW20Id+zgqiMgT3OrZKmWs/Nn7l/kC26TV3MrpduXWQ/liMIkvXhyOAIU0ILA==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "9875800f-c4dd-454c-b73f-9fbaa442f27f",
                            TwoFactorEnabled = false,
                            UserName = "user6@mail.com"
                        },
                        new
                        {
                            Id = "b636ef12-242e-49ec-8f3f-7ba5c39da705",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "55187ebf-ec94-4f67-8e84-8c7374db12d4",
                            Email = "user7@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User7",
                            LastName = "User7",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER7@MAIL.COM",
                            NormalizedUserName = "USER7@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAECmODnnlQwX8NYvx2kwaNRhbZaUxSdNAwv5W9Y7LzyrkEYqKCUKp3dvA6GEfPBzGvQ==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "6afa6219-dd85-48a4-bedf-bfb87aadb4a7",
                            TwoFactorEnabled = false,
                            UserName = "user7@mail.com"
                        },
                        new
                        {
                            Id = "8cd62f7a-bd8a-46e3-9caf-f0b4cf1f0c26",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "cb8e733a-8f8a-448a-946a-ae5d0beeab86",
                            Email = "user8@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User8",
                            LastName = "User8",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER8@MAIL.COM",
                            NormalizedUserName = "USER8@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEDWpk2lylWaEDCaQg2PqzzT6yYmYlgxR9rF6/Eva8DYuWMSBEWdWIqNAaR0laLh6BQ==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "b98c05f4-d9fa-425a-bc11-8165fa4d8e16",
                            TwoFactorEnabled = false,
                            UserName = "user8@mail.com"
                        },
                        new
                        {
                            Id = "4ac29d46-7b8f-485d-b0d7-b49179f8a8f1",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "cc8d3d5e-4de4-4679-819c-2b844785bb39",
                            Email = "user9@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User9",
                            LastName = "User9",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER9@MAIL.COM",
                            NormalizedUserName = "USER9@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEPvy6tM/2cwRfDzsNmuqENPRMqXvfFuVaz5Y9VvjzuzJ7pJNhFb2scLXSQiaPeI9mg==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "0dca498f-2737-475f-a6d9-c5dc17950c21",
                            TwoFactorEnabled = false,
                            UserName = "user9@mail.com"
                        },
                        new
                        {
                            Id = "a94e70ce-b628-46e3-9c12-150147341345",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "36f920fb-8a39-4c30-a293-be783b2b07d9",
                            Email = "user10@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User10",
                            LastName = "User10",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER10@MAIL.COM",
                            NormalizedUserName = "USER10@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAENyDOLvcgL4jReEJZ398gM4TN6c2Tu1MfVC3oIaNo+fiYoTNgSAM4eewh8BjI4fEdQ==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "95947cb8-e538-46d6-98e6-5db318b8f63d",
                            TwoFactorEnabled = false,
                            UserName = "user10@mail.com"
                        },
                        new
                        {
                            Id = "0314e891-80e5-4dd3-b0ba-3416f8397ce8",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "d0c05c57-ef5f-489d-a9c0-af2d6bac21d0",
                            Email = "user11@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User11",
                            LastName = "User11",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER11@MAIL.COM",
                            NormalizedUserName = "USER11@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEGwROIaHp/lebC5C0BENaUw8ZDoLjtld61OG7QlE7Qtacs89Vm9tezNr96quLiP8aQ==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "ec1425cc-ab90-4c53-a762-56a193e9fb05",
                            TwoFactorEnabled = false,
                            UserName = "user11@mail.com"
                        },
                        new
                        {
                            Id = "ea00df27-92c3-4a09-adb8-a6a97bb29487",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "d3d0c752-fb86-4636-bcae-8e76301e259d",
                            Email = "user12@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User12",
                            LastName = "User12",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER12@MAIL.COM",
                            NormalizedUserName = "USER12@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEAu7aV39nBwLHzHnYgG4neZ/kTF6siVvWxgoGwAevMgAWHR3aAtq57CXxoEb96xKeQ==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "df7b1424-1a15-4030-8dbd-d3a9bf91c963",
                            TwoFactorEnabled = false,
                            UserName = "user12@mail.com"
                        },
                        new
                        {
                            Id = "f5f2dfd4-f46c-483a-b4c6-56482b4c0491",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "a4c973cf-12a1-4816-a3ec-92ef9fe0b037",
                            Email = "user13@mail.com",
                            EmailConfirmed = true,
                            FirstName = "User13",
                            LastName = "User13",
                            LockoutEnabled = false,
                            NormalizedEmail = "USER13@MAIL.COM",
                            NormalizedUserName = "USER13@MAIL.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEN2VLaIIkdwo8LX/kzNcqqhUiDruJt52NICYLRh3hGsdHQBcJaAbpmT5YOIahklwig==",
                            PhoneNumber = "0000000000",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "68ad99d0-320c-44ff-b553-bce9ada3860a",
                            TwoFactorEnabled = false,
                            UserName = "user13@mail.com"
                        },
                        new
                        {
                            Id = "dea12856-c198-4129-b3f3-b893d8395082",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "8a929e15-4134-4b0b-90cb-f425df31fa8f",
                            Email = "admin@admin.com",
                            EmailConfirmed = true,
                            FirstName = "Great",
                            LastName = "Admin",
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@ADMIN.COM",
                            NormalizedUserName = "ADMIN@ADMIN.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEHHfj8i4eLWmgRQhmmMR+f+ScUn+DJxYW7YVGHvx+BqiQD6dhg2jyVhHnlIk9Gud1g==",
                            PhoneNumber = "9876543021",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "1dffe681-b79c-4969-8a91-5e674da629d7",
                            TwoFactorEnabled = false,
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
