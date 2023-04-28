namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	using Data.Models;
	using static Data.Constants.SeedConstants;

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
				Id = FirstUserId,
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				NormalizedEmail = "PETAR@MAIL.COM",
				UserName = "petar@mail.com",
				NormalizedUserName = "PETAR@MAIL.COM",
				PhoneNumber = "1234567890",
				EmailConfirmed = true
			};
			user1.PasswordHash = hasher.HashPassword(user1, "petar123");
			users.Add(user1);

			for (int i = 3; i <= 13; i++)
			{
				var user = new ApplicationUser()
				{
					Id = Guid.NewGuid().ToString(),
					FirstName = "User" + i,
					LastName = "User" + i,
					Email = "user" + i + "@mail.com",
					NormalizedEmail = "USER" + i + "@MAIL.COM",
					UserName = "user" + i + "@mail.com",
					NormalizedUserName = "USER" + i + "@MAIL.COM",
					PhoneNumber = "0000000000",
					EmailConfirmed = true
				};
				user.PasswordHash = hasher.HashPassword(user, "user123");
				users.Add(user);
			}

			var admin = new ApplicationUser()
			{
				Id = AdminId,
				FirstName = "Great",
				LastName = "Admin",
				Email = "admin@admin.com",
				NormalizedEmail = "ADMIN@ADMIN.COM",
				UserName = "admin@admin.com",
				NormalizedUserName = "ADMIN@ADMIN.COM",
				PhoneNumber = "9876543021",
				EmailConfirmed = true
			};
			admin.PasswordHash = hasher.HashPassword(admin, "admin123");
			users.Add(admin);

			return users;
		}
	}
}
