using Microsoft.AspNetCore.Identity;
using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using System.Security.Cryptography;
using static PersonalFinancer.Data.Constants;

namespace PersonalFinancer.Web.Infrastructure
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder SeedUserRoles(this IApplicationBuilder app)
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

				ApplicationUser user1 = await userManager.FindByIdAsync(SeedConstants.FirstUserId);
				ApplicationUser user2 = await userManager.FindByIdAsync(SeedConstants.SecondUserId);
				await userManager.AddToRoleAsync(user1, RoleConstants.UserRoleName);
				await userManager.AddToRoleAsync(user2, RoleConstants.UserRoleName);
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
				for (int i = 1; i <= 10; i++)
				{
					await accountService.CreateAccount(new EditAccountFormDTO
					{
						Name = "Account" + i,
						AccountTypeId = SeedConstants.SecondUserCashMoneyAccountTypeId,
						Balance = 0,
						CurrencyId = SeedConstants.SecondUserGBPCurrencyId,
						OwnerId = SeedConstants.SecondUserId
					});
				}

				string cashBgnAccId = await accountService.CreateAccount(new EditAccountFormDTO
				{
					Name = "Cash BGN",
					AccountTypeId = SeedConstants.FirstUserCashAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserBGNCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				string bankBgnAccId = await accountService.CreateAccount(new EditAccountFormDTO
				{
					Name = "Bank BGN",
					AccountTypeId = SeedConstants.FirstUserBankAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserBGNCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				string euroSavingsAccId = await accountService.CreateAccount(new EditAccountFormDTO
				{
					Name = "Euro Savings",
					AccountTypeId = SeedConstants.FirstUserSavingAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserEURCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				string usdSavingsAccId = await accountService.CreateAccount(new EditAccountFormDTO
				{
					Name = "Dolar Savings",
					AccountTypeId = SeedConstants.FirstUserSavingAccountTypeId,
					Balance = 0,
					CurrencyId = SeedConstants.FirstUserUSDCurrencyId,
					OwnerId = SeedConstants.FirstUserId
				});

				await accountService.CreateTransaction(new CreateTransactionInputDTO
				{
					AccountId = cashBgnAccId,
					Amount = 2000,
					OwnerId = SeedConstants.FirstUserId,
					CategoryId = TransactionConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = TransactionConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});
				await accountService.CreateTransaction(new CreateTransactionInputDTO
				{
					AccountId = bankBgnAccId,
					Amount = 4000,
					OwnerId = SeedConstants.FirstUserId,
					CategoryId = TransactionConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = TransactionConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});
				await accountService.CreateTransaction(new CreateTransactionInputDTO
				{
					AccountId = euroSavingsAccId,
					Amount = 2800,
					OwnerId = SeedConstants.FirstUserId,
					CategoryId = TransactionConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = TransactionConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});
				await accountService.CreateTransaction(new CreateTransactionInputDTO
				{
					AccountId = usdSavingsAccId,
					Amount = 3800,
					OwnerId = SeedConstants.FirstUserId,
					CategoryId = TransactionConstants.InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = TransactionConstants.CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});

				int taxiCounter = 0;

				for (int i = 58; i >= 1; i--)
				{
					if (i == 57 || i == 32 || i == 7)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = bankBgnAccId,
							Amount = 1500m,
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.SalaryCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Salary",
							TransactionType = TransactionType.Income
						});
					}

					if (taxiCounter <= 5)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = cashBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.TransportCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Taxi",
							TransactionType = TransactionType.Expense
						});
					}

					if (i == 40 || i == 20)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = bankBgnAccId,
							Amount = 14.99m,
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.MedicalHealthcareCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Vitamins",
							TransactionType = TransactionType.Expense
						});
					}

					if (i == 38 || i == 18)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = euroSavingsAccId,
							Amount = 100,
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.DividentsCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Stocks dividents",
							TransactionType = TransactionType.Income
						});
					}

					if (i == 34 || i == 14)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = usdSavingsAccId,
							Amount = 150,
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.DividentsCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Stocks dividents",
							TransactionType = TransactionType.Income
						});
					}

					if (i == 54 || i == 29 || i == 4)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 4.83, 2),
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Electricity bill",
							TransactionType = TransactionType.Expense
						});
					}

					await accountService.CreateTransaction(new CreateTransactionInputDTO
					{
						AccountId = cashBgnAccId,
						Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.53, 2),
						OwnerId = SeedConstants.FirstUserId,
						CategoryId = SeedConstants.FoodDrinkCategoryId,
						CreatedOn = DateTime.UtcNow.AddDays(-i),
						Refference = "Lunch",
						TransactionType = TransactionType.Expense
					});

					if (i == 52 || i == 27 || i == 2)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 1.83, 2),
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Water bill",
							TransactionType = TransactionType.Expense
						});
					}

					if (i == 50 || i == 25 || i == 1)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 2.83, 2),
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Phone bill",
							TransactionType = TransactionType.Expense
						});
					}

					if (taxiCounter <= 5)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = cashBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.TransportCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Taxi",
							TransactionType = TransactionType.Expense
						});

						if (taxiCounter == 5)
						{
							taxiCounter = 0;
						}
					}

					if (i == 48 || i == 21)
					{
						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = bankBgnAccId,
							Amount = 500,
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.MoneyTransferCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "ATM Withdraw",
							TransactionType = TransactionType.Expense
						});

						await accountService.CreateTransaction(new CreateTransactionInputDTO
						{
							AccountId = cashBgnAccId,
							Amount = 500,
							OwnerId = SeedConstants.FirstUserId,
							CategoryId = SeedConstants.MoneyTransferCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "ATM Withdraw",
							TransactionType = TransactionType.Income
						});
					}

					await accountService.CreateTransaction(new CreateTransactionInputDTO
					{
						AccountId = bankBgnAccId,
						Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.83, 2),
						OwnerId = SeedConstants.FirstUserId,
						CategoryId = SeedConstants.FoodDrinkCategoryId,
						CreatedOn = DateTime.UtcNow.AddDays(-i),
						Refference = "Dinner",
						TransactionType = TransactionType.Expense
					});
				}
			})
				.GetAwaiter()
				.GetResult();

			return app;
		}
	}
}