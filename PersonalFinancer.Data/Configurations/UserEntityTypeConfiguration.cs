﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.SeedConstants;

namespace PersonalFinancer.Data.Configurations
{
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

			var user = new ApplicationUser()
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
			user.PasswordHash = hasher.HashPassword(user, "petar123");
			users.Add(user);

			var admin = new ApplicationUser()
			{
				Id = AdminId,
				FirstName = "Great",
				LastName = "Admin",
				Email = "admin@admin.com",
				NormalizedEmail = "ADMIN@ADMIN.COM",
				UserName = "admin@admin.com",
				NormalizedUserName = "ADMIN@ADMIN.COM",
				PhoneNumber = "0987654321",
				EmailConfirmed = true
			};
			admin.PasswordHash = hasher.HashPassword(admin, "admin123");
			users.Add(admin);

			return users;
		}
	}
}
