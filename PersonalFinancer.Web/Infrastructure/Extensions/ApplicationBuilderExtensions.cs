using Microsoft.AspNetCore.Identity;
using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Models.Enums;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using System.Security.Cryptography;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Web.Infrastructure.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder SeedRoles(this IApplicationBuilder app)
		{
			using IServiceScope scope = app.ApplicationServices.CreateScope();

			IServiceProvider services = scope.ServiceProvider;
			UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
			RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

			Task.Run(async () =>
			{
				if (await roleManager.RoleExistsAsync(RoleConstants.AdminRoleName))
				{
					return;
				}

				IdentityRole adminRole = new IdentityRole { Name = RoleConstants.AdminRoleName };
				IdentityRole userRole = new IdentityRole { Name = RoleConstants.UserRoleName };
				await roleManager.CreateAsync(adminRole);
				await roleManager.CreateAsync(userRole);

				ApplicationUser admin = await userManager.FindByIdAsync(SeedConstants.AdminId);
				await userManager.AddToRoleAsync(admin, RoleConstants.AdminRoleName);

				ApplicationUser firstUser = await userManager.FindByIdAsync(SeedConstants.FirstUserId);
				await userManager.AddToRoleAsync(firstUser, RoleConstants.UserRoleName);
			})
				.GetAwaiter()
				.GetResult();

			return app;
		}

		public static IApplicationBuilder SeedAccountsAndTransactions(this IApplicationBuilder app)
		{
			using IServiceScope scope = app.ApplicationServices.CreateScope();
			IServiceProvider services = scope.ServiceProvider;
			var context = services.GetRequiredService<PersonalFinancerDbContext>();

			if (context.Accounts.Any() || context.Transactions.Any())
			{
				return app;
			}

			IAccountsService accountService = services.GetRequiredService<IAccountsService>();

			Task.Run(async () =>
			{
				string cashBgnAccId = await accountService.CreateAccount(new AccountFormShortServiceModel
				{
					Name = "Cash BGN",
					AccountTypeId = SeedConstants.FirstUserCashAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserBGNCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				string bankBgnAccId = await accountService.CreateAccount(new AccountFormShortServiceModel
				{
					Name = "Bank BGN",
					AccountTypeId = SeedConstants.FirstUserBankAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserBGNCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				string euroSavingsAccId = await accountService.CreateAccount(new AccountFormShortServiceModel
				{
					Name = "Euro Savings",
					AccountTypeId = SeedConstants.FirstUserSavingAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserEURCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				string usdSavingsAccId = await accountService.CreateAccount(new AccountFormShortServiceModel
				{
					Name = "Dolar Savings",
					AccountTypeId = SeedConstants.FirstUserSavingAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserUSDCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				await accountService.CreateTransaction(new TransactionFormShortServiceModel
				{
					AccountId = cashBgnAccId,
					Amount = 2000,
					CategoryId = CategoryConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true,
					OwnerId = SeedConstants.FirstUserId
				});
				await accountService.CreateTransaction(new TransactionFormShortServiceModel
				{
					AccountId = bankBgnAccId,
					Amount = 4000,
					CategoryId = CategoryConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true,
					OwnerId = SeedConstants.FirstUserId
				});
				await accountService.CreateTransaction(new TransactionFormShortServiceModel
				{
					AccountId = euroSavingsAccId,
					Amount = 2800,
					CategoryId = CategoryConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true,
					OwnerId = SeedConstants.FirstUserId
				});
				await accountService.CreateTransaction(new TransactionFormShortServiceModel
				{
					AccountId = usdSavingsAccId,
					Amount = 3800,
					CategoryId = CategoryConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true,
					OwnerId = SeedConstants.FirstUserId
				});

				int taxiCounter = 0;

				for (int i = 58; i >= 1; i--)
				{
					if (i == 57 || i == 32 || i == 7)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = bankBgnAccId,
							Amount = 1500m,
							CategoryId = SeedConstants.SalaryCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Salary",
							TransactionType = TransactionType.Income,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					if (taxiCounter <= 5)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = cashBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
							CategoryId = SeedConstants.TransportCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Taxi",
							TransactionType = TransactionType.Expense,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					if (i == 40 || i == 20)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = bankBgnAccId,
							Amount = 14.99m,
							CategoryId = SeedConstants.MedicalHealthcareCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Vitamins",
							TransactionType = TransactionType.Expense,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					if (i == 38 || i == 18)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = euroSavingsAccId,
							Amount = 100,
							CategoryId = SeedConstants.DividentsCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Stocks dividents",
							TransactionType = TransactionType.Income,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					if (i == 34 || i == 14)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = usdSavingsAccId,
							Amount = 150,
							CategoryId = SeedConstants.DividentsCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Stocks dividents",
							TransactionType = TransactionType.Income,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					if (i == 54 || i == 29 || i == 4)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 4.83, 2),
							CategoryId = SeedConstants.UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Electricity bill",
							TransactionType = TransactionType.Expense,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					await accountService.CreateTransaction(new TransactionFormShortServiceModel
					{
						AccountId = cashBgnAccId,
						Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.53, 2),
						CategoryId = SeedConstants.FoodDrinkCategoryId,
						CreatedOn = DateTime.UtcNow.AddDays(-i),
						Refference = "Lunch",
						TransactionType = TransactionType.Expense,
						OwnerId = SeedConstants.FirstUserId
					});

					if (i == 52 || i == 27 || i == 2)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 1.83, 2),
							CategoryId = SeedConstants.UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Water bill",
							TransactionType = TransactionType.Expense,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					if (i == 50 || i == 25 || i == 1)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 2.83, 2),
							CategoryId = SeedConstants.UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Phone bill",
							TransactionType = TransactionType.Expense,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					if (taxiCounter <= 5)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = cashBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
							CategoryId = SeedConstants.TransportCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Taxi",
							TransactionType = TransactionType.Expense,
							OwnerId = SeedConstants.FirstUserId
						});

						if (taxiCounter == 5)
						{
							taxiCounter = 0;
						}
					}

					if (i == 48 || i == 21)
					{
						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = bankBgnAccId,
							Amount = 500,
							CategoryId = SeedConstants.MoneyTransferCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "ATM Withdraw",
							TransactionType = TransactionType.Expense,
							OwnerId = SeedConstants.FirstUserId
						});

						await accountService.CreateTransaction(new TransactionFormShortServiceModel
						{
							AccountId = cashBgnAccId,
							Amount = 500,
							CategoryId = SeedConstants.MoneyTransferCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "ATM Withdraw",
							TransactionType = TransactionType.Income,
							OwnerId = SeedConstants.FirstUserId
						});
					}

					await accountService.CreateTransaction(new TransactionFormShortServiceModel
					{
						AccountId = bankBgnAccId,
						Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.83, 2),
						CategoryId = SeedConstants.FoodDrinkCategoryId,
						CreatedOn = DateTime.UtcNow.AddDays(-i),
						Refference = "Dinner",
						TransactionType = TransactionType.Expense,
						OwnerId = SeedConstants.FirstUserId
					});
				}
			})
				.GetAwaiter()
				.GetResult();

			return app;
		}
	}
}