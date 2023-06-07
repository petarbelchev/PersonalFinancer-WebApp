using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.SeedConstants;

namespace PersonalFinancer.Data.Seeding
{
	internal class UsersSeedConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.HasData(SeedUsers());
		}

		private static IEnumerable<ApplicationUser> SeedUsers()
		{
			var hasher = new PasswordHasher<ApplicationUser>();
			var users = new List<ApplicationUser>();

			var user = new ApplicationUser()
			{
				Id = FirstUserId,
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				NormalizedEmail = "PETAR@MAIL.COM",
				UserName = "petar2023",
				NormalizedUserName = "PETAR2023",
				PhoneNumber = "1234567890",
				EmailConfirmed = true
			};
			user.PasswordHash = hasher.HashPassword(user, "petar123");
			users.Add(user);

			var admin = new ApplicationUser()
			{
				Id = AdminId,
				FirstName = "Great",
				LastName = "Admin",
				Email = "admin@admin.com",
				NormalizedEmail = "ADMIN@ADMIN.COM",
				UserName = "admin",
				NormalizedUserName = "ADMIN",
				PhoneNumber = "0987654321",
				EmailConfirmed = true
			};
			admin.PasswordHash = hasher.HashPassword(admin, "admin123");
			users.Add(admin);

			return users;
		}
	}
}
