using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;
using static PersonalFinancer.Data.Constants.SeedConstants;
using static PersonalFinancer.Data.Constants.TransactionConstants;

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
				if (await roleManager.RoleExistsAsync(AdminRoleName))
				{
					return;
				}

				IdentityRole adminRole = new IdentityRole { Name = AdminRoleName };
				IdentityRole userRole = new IdentityRole { Name = UserRoleName };
				await roleManager.CreateAsync(adminRole);
				await roleManager.CreateAsync(userRole);

				ApplicationUser admin = await userManager.FindByIdAsync(AdminId);
				await userManager.AddToRoleAsync(admin, AdminRoleName);

				ApplicationUser user1 = await userManager.FindByIdAsync(FirstUserId);
				ApplicationUser user2 = await userManager.FindByIdAsync(SecondUserId);
				await userManager.AddToRoleAsync(user1, UserRoleName);
				await userManager.AddToRoleAsync(user2, UserRoleName);
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

			ITransactionsService transactionsService = services.GetRequiredService<ITransactionsService>();
			IAccountService accountService = services.GetRequiredService<IAccountService>();

			Task.Run(async () =>
			{
				string cashBgnAccId = await accountService.CreateAccount(new AccountFormModel
				{
					Name = "Cash BGN",
					AccountTypeId = CashAccountTypeId,
					Balance = 0,
					CurrencyId = BgnCurrencyId,
					OwnerId = FirstUserId
				});

				string bankBgnAccId = await accountService.CreateAccount(new AccountFormModel
				{
					Name = "Bank BGN",
					AccountTypeId = BankAccountTypeId,
					Balance = 0,
					CurrencyId = BgnCurrencyId,
					OwnerId = FirstUserId
				});

				string euroSavingsAccId = await accountService.CreateAccount(new AccountFormModel
				{
					Name = "Euro Savings",
					AccountTypeId = SavingAccountTypeId,
					Balance = 0,
					CurrencyId = EurCurrencyId,
					OwnerId = FirstUserId
				});

				string usdSavingsAccId = await accountService.CreateAccount(new AccountFormModel
				{
					Name = "Dolar Savings",
					AccountTypeId = SavingAccountTypeId,
					Balance = 0,
					CurrencyId = UsdCurrencyId,
					OwnerId = FirstUserId
				});

				await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
				{
					AccountId = cashBgnAccId,
					Amount = 2000,
					CategoryId = InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});
				await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
				{
					AccountId = bankBgnAccId,
					Amount = 4000,
					CategoryId = InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});
				await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
				{
					AccountId = euroSavingsAccId,
					Amount = 2800,
					CategoryId = InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});
				await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
				{
					AccountId = usdSavingsAccId,
					Amount = 3800,
					CategoryId = InitialBalanceCategoryId,
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income,
					IsInitialBalance = true
				});

				int taxiCounter = 0;

				for (int i = 58; i >= 1; i--)
				{
					if (i == 57 || i == 32 || i == 7)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = bankBgnAccId,
							Amount = 1500m,
							CategoryId = SalaryCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Salary",
							TransactionType = TransactionType.Income
						});
					}

					if (taxiCounter <= 5)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = cashBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
							CategoryId = TransportCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Taxi",
							TransactionType = TransactionType.Expense
						});
					}

					if (i == 40 || i == 20)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = bankBgnAccId,
							Amount = 14.99m,
							CategoryId = MedicalHealthcareCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Vitamins",
							TransactionType = TransactionType.Expense
						});
					}

					if (i == 38 || i == 18)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = euroSavingsAccId,
							Amount = 100,
							CategoryId = DividentsCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Stocks dividents",
							TransactionType = TransactionType.Income
						});
					}

					if (i == 34 || i == 14)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = usdSavingsAccId,
							Amount = 150,
							CategoryId = DividentsCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Stocks dividents",
							TransactionType = TransactionType.Income
						});
					}

					if (i == 54 || i == 29 || i == 4)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 4.83, 2),
							CategoryId = UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Electricity bill",
							TransactionType = TransactionType.Expense
						});
					}

					await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
					{
						AccountId = cashBgnAccId,
						Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.53, 2),
						CategoryId = FoodDrinkCategoryId,
						CreatedOn = DateTime.UtcNow.AddDays(-i),
						Refference = "Lunch",
						TransactionType = TransactionType.Expense
					});

					if (i == 52 || i == 27 || i == 2)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 1.83, 2),
							CategoryId = UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Water bill",
							TransactionType = TransactionType.Expense
						});
					}

					if (i == 50 || i == 25 || i == 1)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = bankBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 2.83, 2),
							CategoryId = UtilitiesCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "Phone bill",
							TransactionType = TransactionType.Expense
						});
					}

					if (taxiCounter <= 5)
					{
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = cashBgnAccId,
							Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
							CategoryId = TransportCategoryId,
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
						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = bankBgnAccId,
							Amount = 500,
							CategoryId = MoneyTransferCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "ATM Withdraw",
							TransactionType = TransactionType.Expense
						});

						await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
						{
							AccountId = cashBgnAccId,
							Amount = 500,
							CategoryId = MoneyTransferCategoryId,
							CreatedOn = DateTime.UtcNow.AddDays(-i),
							Refference = "ATM Withdraw",
							TransactionType = TransactionType.Income
						});
					}

					await transactionsService.CreateTransaction(FirstUserId, new TransactionFormModel
					{
						AccountId = bankBgnAccId,
						Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.83, 2),
						CategoryId = FoodDrinkCategoryId,
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