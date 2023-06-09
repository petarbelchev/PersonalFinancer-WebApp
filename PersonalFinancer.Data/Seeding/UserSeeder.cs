namespace PersonalFinancer.Data.Seeding
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using PersonalFinancer.Data.Models;
    using System;
    using System.Threading.Tasks;
    using static PersonalFinancer.Data.Constants;

    public class UserSeeder : ISeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
        {
            UserManager<ApplicationUser> userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationUser? admin = await userManager.FindByEmailAsync(SeedConstants.AdminEmail);

            if (admin != null)
                return;

            admin = new ApplicationUser
            {
                FirstName = "Great",
                LastName = "Admin",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "0987654321",
                EmailConfirmed = true,
            };

            IdentityResult creationResult =
               await userManager.CreateAsync(admin, SeedConstants.AdminPassword);

            if (creationResult.Succeeded)
            {
                _ = await userManager.AddToRoleAsync(admin, RoleConstants.AdminRoleName);

                _ = dbContext.Categories.Add(new Category
                {
                    Id = Guid.Parse(CategoryConstants.InitialBalanceCategoryId),
                    Name = CategoryConstants.CategoryInitialBalanceName,
                    OwnerId = admin.Id,
                });

                _ = await dbContext.SaveChangesAsync();
            }

            var firstUser = new ApplicationUser
            {
                FirstName = "Petar",
                LastName = "Petrov",
                Email = "petar@mail.com",
                NormalizedEmail = "PETAR@MAIL.COM",
                UserName = "petar",
                NormalizedUserName = "PETAR2023",
                PhoneNumber = "1234567890",
                EmailConfirmed = true,
            };

            creationResult = await userManager.CreateAsync(firstUser, SeedConstants.FirstUserPassword);

            if (creationResult.Succeeded)
                _ = await userManager.AddToRoleAsync(firstUser, RoleConstants.UserRoleName);
        }
    }
}
