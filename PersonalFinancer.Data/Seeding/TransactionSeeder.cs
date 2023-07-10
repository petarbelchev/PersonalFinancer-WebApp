﻿namespace PersonalFinancer.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using System.Security.Cryptography;
	using static PersonalFinancer.Common.Constants.CategoryConstants;
	using static PersonalFinancer.Common.Constants.SeedConstants;

	public class TransactionSeeder : IUserDataSeeder
    {
        public async Task SeedAsync(PersonalFinancerDbContext dbContext, ApplicationUser user)
		{
			if (await dbContext.Accounts.AnyAsync(a => a.OwnerId == user.Id && a.Balance != 0))
				return;

			var cashBgnAccId = Guid.Parse(CashBgnAccountId);
            var bankBgnAccId = Guid.Parse(BankBgnAccountId);
            var euroSavingsAccId = Guid.Parse(BankEurAccountId);
            var usdSavingsAccId = Guid.Parse(BankUsdAccountId);

            var initialBalanceCategoryId = Guid.Parse(InitialBalanceCategoryId);
            var salaryCategoryId = Guid.Parse(SalaryCategoryId);
            var transportCategoryId = Guid.Parse(TransportCategoryId);
            var medicalHealthcareCategoryId = Guid.Parse(MedicalHealthcareCategoryId);
            var dividendsCategoryId = Guid.Parse(DividendsCategoryId);
            var utilitiesCategoryId = Guid.Parse(UtilitiesCategoryId);
            var moneyTransferCategoryId = Guid.Parse(MoneyTransferCategoryId);
            var foodDrinkCategoryId = Guid.Parse(FoodDrinkCategoryId);

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = cashBgnAccId,
                    Amount = 2000,
                    CategoryId = initialBalanceCategoryId,
                    CreatedOnUtc = DateTime.UtcNow.AddMonths(-2),
                    Reference = CategoryInitialBalanceName,
                    TransactionType = TransactionType.Income,
                    IsInitialBalance = true,
                    OwnerId = user.Id,
                },

                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = bankBgnAccId,
                    Amount = 4000,
                    CategoryId = initialBalanceCategoryId,
                    CreatedOnUtc = DateTime.UtcNow.AddMonths(-2),
                    Reference = CategoryInitialBalanceName,
                    TransactionType = TransactionType.Income,
                    IsInitialBalance = true,
                    OwnerId = user.Id,
                },

                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = euroSavingsAccId,
                    Amount = 2800,
                    CategoryId = initialBalanceCategoryId,
                    CreatedOnUtc = DateTime.UtcNow.AddMonths(-2),
                    Reference = CategoryInitialBalanceName,
                    TransactionType = TransactionType.Income,
                    IsInitialBalance = true,
                    OwnerId = user.Id,
                },

                new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = usdSavingsAccId,
                    Amount = 3800,
                    CategoryId = initialBalanceCategoryId,
                    CreatedOnUtc = DateTime.UtcNow.AddMonths(-2),
                    Reference = CategoryInitialBalanceName,
                    TransactionType = TransactionType.Income,
                    IsInitialBalance = true,
                    OwnerId = user.Id,
                }
            };

            int taxiCounter = 0;

            for (int i = 58; i >= 1; i--)
            {
                if (i == 57 || i == 32 || i == 7)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = bankBgnAccId,
                        Amount = 1500m,
                        CategoryId = salaryCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Salary",
                        TransactionType = TransactionType.Income,
                        OwnerId = user.Id,
                    });
                }

                if (taxiCounter <= 5)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = cashBgnAccId,
                        Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
                        CategoryId = transportCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Taxi",
                        TransactionType = TransactionType.Expense,
                        OwnerId = user.Id,
                    });
                }

                if (i == 40 || i == 20)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = bankBgnAccId,
                        Amount = 14.99m,
                        CategoryId = medicalHealthcareCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Vitamins",
                        TransactionType = TransactionType.Expense,
                        OwnerId = user.Id,
                    });
                }

                if (i == 38 || i == 18)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = euroSavingsAccId,
                        Amount = 100,
                        CategoryId = dividendsCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Stocks dividends",
                        TransactionType = TransactionType.Income,
                        OwnerId = user.Id,
                    });
                }

                if (i == 34 || i == 14)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = usdSavingsAccId,
                        Amount = 150,
                        CategoryId = dividendsCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Stocks dividends",
                        TransactionType = TransactionType.Income,
                        OwnerId = user.Id,
                    });
                }

                if (i == 54 || i == 29 || i == 4)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = bankBgnAccId,
                        Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 4.83, 2),
                        CategoryId = utilitiesCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Electricity bill",
                        TransactionType = TransactionType.Expense,
                        OwnerId = user.Id,
                    });
                }

                transactions.Add(new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = cashBgnAccId,
                    Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.53, 2),
                    CategoryId = foodDrinkCategoryId,
                    CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                    Reference = "Lunch",
                    TransactionType = TransactionType.Expense,
                    OwnerId = user.Id,
                });

                if (i == 52 || i == 27 || i == 2)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = bankBgnAccId,
                        Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 1.83, 2),
                        CategoryId = utilitiesCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Water bill",
                        TransactionType = TransactionType.Expense,
                        OwnerId = user.Id,
                    });
                }

                if (i == 50 || i == 25 || i == 1)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = bankBgnAccId,
                        Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 2.83, 2),
                        CategoryId = utilitiesCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Phone bill",
                        TransactionType = TransactionType.Expense,
                        OwnerId = user.Id,
                    });
                }

                if (taxiCounter <= 5)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = cashBgnAccId,
                        Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.63, 2),
                        CategoryId = transportCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "Taxi",
                        TransactionType = TransactionType.Expense,
                        OwnerId = user.Id,
                    });

                    if (taxiCounter == 5)
                    {
                        taxiCounter = 0;
                    }
                }

                if (i == 48 || i == 21)
                {
                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = bankBgnAccId,
                        Amount = 500,
                        CategoryId = moneyTransferCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "ATM Withdraw",
                        TransactionType = TransactionType.Expense,
                        OwnerId = user.Id,
                    });

                    transactions.Add(new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = cashBgnAccId,
                        Amount = 500,
                        CategoryId = moneyTransferCategoryId,
                        CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                        Reference = "ATM Withdraw",
                        TransactionType = TransactionType.Income,
                        OwnerId = user.Id,
                    });
                }

                transactions.Add(new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = bankBgnAccId,
                    Amount = (decimal)Math.Round(RandomNumberGenerator.GetInt32(9, 17) * 0.83, 2),
                    CategoryId = foodDrinkCategoryId,
                    CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                    Reference = "Dinner",
                    TransactionType = TransactionType.Expense,
                    OwnerId = user.Id,
                });
            }

            foreach (Account account in dbContext.Accounts)
                account.Balance += CalculateBalance(account.Id, transactions);

            await dbContext.Transactions.AddRangeAsync(transactions);
            await dbContext.SaveChangesAsync();
        }

        private static decimal CalculateBalance(Guid accountId, List<Transaction> transactions)
        {
            decimal incomes = transactions
                .Where(t => t.AccountId == accountId && t.TransactionType == TransactionType.Income)
                .Sum(t => t.Amount);

            decimal expenses = transactions
                .Where(t => t.AccountId == accountId && t.TransactionType == TransactionType.Expense)
                .Sum(t => t.Amount);

            return incomes - expenses;
        }
    }
}
