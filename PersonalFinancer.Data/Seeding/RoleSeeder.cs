namespace PersonalFinancer.Data.Seeding
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;
    using static PersonalFinancer.Data.Constants;

    public class RoleSeeder : ISeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole<Guid>> roleManager =
               serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            if (await roleManager.RoleExistsAsync(RoleConstants.AdminRoleName))
                return;

            var adminRole = new IdentityRole<Guid> { Name = RoleConstants.AdminRoleName };
            var userRole = new IdentityRole<Guid> { Name = RoleConstants.UserRoleName };
            _ = await roleManager.CreateAsync(adminRole);
            _ = await roleManager.CreateAsync(userRole);
        }
    }
}
