using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancer.Data.Migrations
{
    public partial class RemovedSeededUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0314e891-80e5-4dd3-b0ba-3416f8397ce8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3f9c0798-5ef7-4512-81f3-8de9f815e762");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4ac29d46-7b8f-485d-b0d7-b49179f8a8f1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7abc426f-0545-4c45-bb62-ce6332f0ccb0");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8cd62f7a-bd8a-46e3-9caf-f0b4cf1f0c26");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a94e70ce-b628-46e3-9c12-150147341345");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b636ef12-242e-49ec-8f3f-7ba5c39da705");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4cd6b5a-59ae-4878-bbd3-e07a85229b8a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ea00df27-92c3-4a09-adb8-a6a97bb29487");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f5f2dfd4-f46c-483a-b4c6-56482b4c0491");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fd79173d-0dbd-4d1b-b628-bf96c1a4c93d");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9722696-4596-4884-a4d3-0b615c4f56c7", "AQAAAAEAACcQAAAAEEkLIGo3ZOHw2HcWEm1YRHXjVyu4jmq6HoBLS5gRFYQmu8MZaQSOVS0stWUmE6WCfA==", "b23d03ec-29c8-4d52-89ed-802a1ff0ec98" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "33c1a3bb-c587-462c-a947-676193ab7e7c", "AQAAAAEAACcQAAAAEDE2b18osPUFCLCYOO918KGqw3yGoVX2OQzRpUn21xv5sbKMP7nMZyPp6lrnrO+YIA==", "0987654321", "1594438b-3b2e-4fac-8e5b-dbed2aff5f16" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b12a35c7-dcec-40e0-b8aa-f0d5976f50a5", "AQAAAAEAACcQAAAAEJVaHTFHlCHicZjaaEuOggMSz69lLD0g7C0dRhi9S30xCO18WMRS0WEPM/Un2Au/SQ==", "20796eb1-107c-4e41-b67f-30404af5b616" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "8a929e15-4134-4b0b-90cb-f425df31fa8f", "AQAAAAEAACcQAAAAEHHfj8i4eLWmgRQhmmMR+f+ScUn+DJxYW7YVGHvx+BqiQD6dhg2jyVhHnlIk9Gud1g==", "9876543021", "1dffe681-b79c-4969-8a91-5e674da629d7" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0314e891-80e5-4dd3-b0ba-3416f8397ce8", 0, "d0c05c57-ef5f-489d-a9c0-af2d6bac21d0", "user11@mail.com", true, "User11", "User11", false, null, "USER11@MAIL.COM", "USER11@MAIL.COM", "AQAAAAEAACcQAAAAEGwROIaHp/lebC5C0BENaUw8ZDoLjtld61OG7QlE7Qtacs89Vm9tezNr96quLiP8aQ==", "0000000000", false, "ec1425cc-ab90-4c53-a762-56a193e9fb05", false, "user11@mail.com" },
                    { "3f9c0798-5ef7-4512-81f3-8de9f815e762", 0, "07529379-7165-4691-856e-b33dadecc1c6", "user6@mail.com", true, "User6", "User6", false, null, "USER6@MAIL.COM", "USER6@MAIL.COM", "AQAAAAEAACcQAAAAEJf0mjW20Id+zgqiMgT3OrZKmWs/Nn7l/kC26TV3MrpduXWQ/liMIkvXhyOAIU0ILA==", "0000000000", false, "9875800f-c4dd-454c-b73f-9fbaa442f27f", false, "user6@mail.com" },
                    { "4ac29d46-7b8f-485d-b0d7-b49179f8a8f1", 0, "cc8d3d5e-4de4-4679-819c-2b844785bb39", "user9@mail.com", true, "User9", "User9", false, null, "USER9@MAIL.COM", "USER9@MAIL.COM", "AQAAAAEAACcQAAAAEPvy6tM/2cwRfDzsNmuqENPRMqXvfFuVaz5Y9VvjzuzJ7pJNhFb2scLXSQiaPeI9mg==", "0000000000", false, "0dca498f-2737-475f-a6d9-c5dc17950c21", false, "user9@mail.com" },
                    { "7abc426f-0545-4c45-bb62-ce6332f0ccb0", 0, "b78d2b38-5e71-4d3d-a5ca-cc90823fb668", "user3@mail.com", true, "User3", "User3", false, null, "USER3@MAIL.COM", "USER3@MAIL.COM", "AQAAAAEAACcQAAAAEOFzA92DjwAsX84EYmKQhNNkAVbKkVIUpC1Uy2OzsJNxUtPVG8IdjC9WCCbeBwYMOg==", "0000000000", false, "20a15185-ef58-4de6-9af2-af6023bdd25c", false, "user3@mail.com" },
                    { "8cd62f7a-bd8a-46e3-9caf-f0b4cf1f0c26", 0, "cb8e733a-8f8a-448a-946a-ae5d0beeab86", "user8@mail.com", true, "User8", "User8", false, null, "USER8@MAIL.COM", "USER8@MAIL.COM", "AQAAAAEAACcQAAAAEDWpk2lylWaEDCaQg2PqzzT6yYmYlgxR9rF6/Eva8DYuWMSBEWdWIqNAaR0laLh6BQ==", "0000000000", false, "b98c05f4-d9fa-425a-bc11-8165fa4d8e16", false, "user8@mail.com" },
                    { "a94e70ce-b628-46e3-9c12-150147341345", 0, "36f920fb-8a39-4c30-a293-be783b2b07d9", "user10@mail.com", true, "User10", "User10", false, null, "USER10@MAIL.COM", "USER10@MAIL.COM", "AQAAAAEAACcQAAAAENyDOLvcgL4jReEJZ398gM4TN6c2Tu1MfVC3oIaNo+fiYoTNgSAM4eewh8BjI4fEdQ==", "0000000000", false, "95947cb8-e538-46d6-98e6-5db318b8f63d", false, "user10@mail.com" },
                    { "b636ef12-242e-49ec-8f3f-7ba5c39da705", 0, "55187ebf-ec94-4f67-8e84-8c7374db12d4", "user7@mail.com", true, "User7", "User7", false, null, "USER7@MAIL.COM", "USER7@MAIL.COM", "AQAAAAEAACcQAAAAECmODnnlQwX8NYvx2kwaNRhbZaUxSdNAwv5W9Y7LzyrkEYqKCUKp3dvA6GEfPBzGvQ==", "0000000000", false, "6afa6219-dd85-48a4-bedf-bfb87aadb4a7", false, "user7@mail.com" },
                    { "d4cd6b5a-59ae-4878-bbd3-e07a85229b8a", 0, "bdeb8c87-39fb-4364-acdd-0c4ff0322cf7", "user4@mail.com", true, "User4", "User4", false, null, "USER4@MAIL.COM", "USER4@MAIL.COM", "AQAAAAEAACcQAAAAEAmIWv1Zu/FfPfj284yuOQoc7/52UckT+kgB6JGlCWbYbP1JtYjvU3oZA0WDT6sL9A==", "0000000000", false, "7855fcdb-761e-4fc9-873b-dcc1804e8063", false, "user4@mail.com" },
                    { "ea00df27-92c3-4a09-adb8-a6a97bb29487", 0, "d3d0c752-fb86-4636-bcae-8e76301e259d", "user12@mail.com", true, "User12", "User12", false, null, "USER12@MAIL.COM", "USER12@MAIL.COM", "AQAAAAEAACcQAAAAEAu7aV39nBwLHzHnYgG4neZ/kTF6siVvWxgoGwAevMgAWHR3aAtq57CXxoEb96xKeQ==", "0000000000", false, "df7b1424-1a15-4030-8dbd-d3a9bf91c963", false, "user12@mail.com" },
                    { "f5f2dfd4-f46c-483a-b4c6-56482b4c0491", 0, "a4c973cf-12a1-4816-a3ec-92ef9fe0b037", "user13@mail.com", true, "User13", "User13", false, null, "USER13@MAIL.COM", "USER13@MAIL.COM", "AQAAAAEAACcQAAAAEN2VLaIIkdwo8LX/kzNcqqhUiDruJt52NICYLRh3hGsdHQBcJaAbpmT5YOIahklwig==", "0000000000", false, "68ad99d0-320c-44ff-b553-bce9ada3860a", false, "user13@mail.com" },
                    { "fd79173d-0dbd-4d1b-b628-bf96c1a4c93d", 0, "a0140fda-24c5-4b9f-bfc1-7c894b47ff39", "user5@mail.com", true, "User5", "User5", false, null, "USER5@MAIL.COM", "USER5@MAIL.COM", "AQAAAAEAACcQAAAAEFOgjgGhFIITusqfdcd31ZgBH4Cc5ieMo9SoMhc2CgI+b9HL+UEsmwMwdBk9FSf2vw==", "0000000000", false, "76bda890-7a3c-4708-aea2-c7789d377798", false, "user5@mail.com" }
                });
        }
    }
}
