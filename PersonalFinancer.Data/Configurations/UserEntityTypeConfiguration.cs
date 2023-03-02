namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	
	using Models;

	internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.HasData(SeedUsers());
		}

		private IEnumerable<ApplicationUser> SeedUsers()
		{
			var hasher = new PasswordHasher<ApplicationUser>();

			var users = new List<ApplicationUser>();

			var user1 = new ApplicationUser()
			{
				Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				NormalizedEmail = "PETAR@MAIL.COM",
				UserName = "petar@mail.com",
				NormalizedUserName = "PETAR@MAIL.COM",
				PhoneNumber = "1234567890"
			};

			user1.PasswordHash = hasher.HashPassword(user1, "petar123");

			users.Add(user1);

			var user2 = new ApplicationUser()
			{
				Id = "bcb4f072-ecca-43c9-ab26-c060c6f364e4",
				FirstName = "Teodor",
				LastName = "Lesly",
				Email = "teodor@mail.com",
				NormalizedEmail = "TEODOR@MAIL.COM",
				UserName = "teodor@mail.com",
				NormalizedUserName = "TEODOR@MAIL.COM",
				PhoneNumber = "1325476980"
			};

			user2.PasswordHash = hasher.HashPassword(user2, "teodor123");

			users.Add(user2);

			var admin = new ApplicationUser()
			{
				Id = "dea12856-c198-4129-b3f3-b893d8395082",
				FirstName = "Great",
				LastName = "Admin",
				Email = "admin@admin.com",
				NormalizedEmail = "ADMIN@ADMIN.COM",
				UserName = "admin@admin.com",
				NormalizedUserName = "ADMIN@ADMIN.COM",
				PhoneNumber = "9876543021"
			};

			admin.PasswordHash = hasher.HashPassword(admin, "admin123");

			users.Add(admin);

			return users;
		}
	}
}
