using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Data.Configurations
{
	internal class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			var hasher = new PasswordHasher<ApplicationUser>();

			var admin = new ApplicationUser
			{
				Id = "2c990863-a80e-4d7e-b372-115fe0dceace",
				FirstName = "Petar",
				LastName = "Hristov",
				Email = "petar_hristov@mail.com",
				NormalizedEmail = "PETAR_HRISTOV@MAIL.COM",
				PhoneNumber = "+359111111111",
				UserName = "admin",
				NormalizedUserName = "ADMIN"
			};

			admin.PasswordHash = hasher.HashPassword(admin, "admin123");

			var user = new ApplicationUser
			{
				Id = "2e8ce625-1278-4368-87c5-9c79fd7692a4",
				FirstName = "Ivan",
				LastName = "Ivanov",
				Email = "ivan.ivanov@abv.bg",
				NormalizedEmail = "IVAN.IVANOV@ABV.BG",
				PhoneNumber = "+359222222222",
				UserName = "regularUser",
				NormalizedUserName = "REGULARUSER"
			};

			user.PasswordHash = hasher.HashPassword(user, "user123");

			builder.HasData(admin, user);
		}
	}
}
