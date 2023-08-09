namespace PersonalFinancer.Tests.Services
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Distributed;
	using Moq;
	using NUnit.Framework;
	using NUnit.Framework.Internal;
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services;
	using static PersonalFinancer.Common.Constants.CategoryConstants;

	[TestFixture]
	internal abstract class ServicesUnitTestsBase : UnitTestsBase
	{
		protected Guid mainTestUserId;
		protected Guid adminId;

		protected PersonalFinancerDbContext dbContext;
		protected IMapper mapper;
		protected Mock<IDistributedCache> cacheMock;

		[SetUp]
		protected async Task SetUpBase()
		{
			var dbContextOptionsBuilder = new DbContextOptionsBuilder<PersonalFinancerDbContext>()
				.UseInMemoryDatabase("PersonalFinancerInMemoryDb" + DateTime.Now.Ticks.ToString()).Options;
			this.dbContext = new PersonalFinancerDbContext(dbContextOptionsBuilder);

			var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceMappingProfile>());
			this.mapper = config.CreateMapper();

			this.cacheMock = new Mock<IDistributedCache>();

			await this.SeedDatabase();

			this.mainTestUserId = await this.dbContext.Users
				.Where(u => u.Transactions.Any() && !u.IsAdmin)
				.Select(u => u.Id)
				.FirstAsync();

			this.adminId = await this.dbContext.Users
				.Where(u => u.IsAdmin)
				.Select(u => u.Id)
				.FirstAsync();
		}

		[TearDown]
		protected async Task TearDownBase()
			=> await this.dbContext.DisposeAsync();

		private async Task SeedDatabase()
		{
			var users = new List<ApplicationUser>();

			ApplicationUser admin = GetAdmin();
			users.Add(admin);

			int usersCount = 2;
			for (int userNum = 0; userNum < usersCount; userNum++)
			{
				ApplicationUser user = GetUser(userNum);

				AddAccountTypes(user, count: 2, areTheyDeleted: false);
				AddAccountTypes(user, count: 2, areTheyDeleted: true);

				AddCurrencies(user, count: 2, areTheyDeleted: false);
				AddCurrencies(user, count: 2, areTheyDeleted: true);

				AddCategories(user, count: 2, areTheyDeleted: false);
				AddCategories(user, count: 2, areTheyDeleted: true);

				var accountTypesIdsWithAccounts = new List<Guid>
				{
					user.AccountTypes.Where(at => !at.IsDeleted).Select(at => at.Id).First(),
					user.AccountTypes.Where(at => at.IsDeleted).Select(at => at.Id).First()
				};

				var currenciesIdsWithAccounts = new List<Guid>
				{
					user.Currencies.Where(c => !c.IsDeleted).Select(c => c.Id).First(),
					user.Currencies.Where(c => c.IsDeleted).Select(c => c.Id).First()
				};

				AddAccounts(user, count: 3, accountTypesIdsWithAccounts, currenciesIdsWithAccounts, areTheyDeleted: false);
				AddAccounts(user, count: 2, accountTypesIdsWithAccounts, currenciesIdsWithAccounts, areTheyDeleted: true);

				var accountsWithTransactions = new List<Account>();
				accountsWithTransactions.AddRange(user.Accounts.Where(a => !a.IsDeleted).Take(2));
				accountsWithTransactions.Add(user.Accounts.Where(c => c.IsDeleted).First());

				var categoriesIdsWithTransactions = new List<Guid>
				{
					user.Categories.Where(c => c.IsDeleted).Select(c => c.Id).First(),
					user.Categories.Where(c => !c.IsDeleted).Select(c => c.Id).First()
				};

				AddAccountsTransactions(accountsWithTransactions, categoriesIdsWithTransactions);

				users.Add(user);
			}

			await this.dbContext.Users.AddRangeAsync(users);
			await this.dbContext.SaveChangesAsync();
		}

		private static ApplicationUser GetAdmin()
		{
			var admin = new ApplicationUser
			{
				Id = Guid.NewGuid(),
				UserName = "admin",
				NormalizedUserName = "ADMIN",
				FirstName = "Great",
				LastName = "Admin",
				Email = "admin@mail.com",
				NormalizedEmail = "ADMIN@MAIL.COM",
				IsAdmin = true
			};

			var initialBalanceCategory = new Category
			{
				Id = Guid.Parse(InitialBalanceCategoryId),
				Name = CategoryInitialBalanceName,
				OwnerId = admin.Id
			};

			admin.Categories.Add(initialBalanceCategory);

			return admin;
		}

		private static ApplicationUser GetUser(int userNumber)
		{
			var user = new ApplicationUser
			{
				Id = Guid.NewGuid(),
				UserName = "user" + userNumber,
				NormalizedUserName = "USER" + userNumber,
				FirstName = "FirstName" + userNumber,
				LastName = "LastName" + userNumber,
				Email = $"user{userNumber}@mail.com",
				NormalizedEmail = $"USER{userNumber}@MAIL.COM"
			};

			return user;
		}

		private static void AddAccountTypes(
			ApplicationUser user,
			int count,
			bool areTheyDeleted)
		{
			for (int accountTypeNum = 0; accountTypeNum < count; accountTypeNum++)
			{
				var accountType = new AccountType
				{
					Id = Guid.NewGuid(),
					Name = $"{user.UserName}-AccountType{accountTypeNum}" + (areTheyDeleted ? "-Deleted" : string.Empty),
					OwnerId = user.Id,
					IsDeleted = areTheyDeleted
				};

				user.AccountTypes.Add(accountType);
			}
		}

		private static void AddCurrencies(
			ApplicationUser user,
			int count,
			bool areTheyDeleted)
		{
			for (int currencyNum = 0; currencyNum < count; currencyNum++)
			{
				var currency = new Currency
				{
					Id = Guid.NewGuid(),
					Name = $"{user.UserName}-Currency{currencyNum}" + (areTheyDeleted ? "-Deleted" : string.Empty),
					OwnerId = user.Id,
					IsDeleted = areTheyDeleted
				};

				user.Currencies.Add(currency);
			}
		}

		private static void AddCategories(
			ApplicationUser user,
			int count,
			bool areTheyDeleted)
		{
			for (int categoryNum = 0; categoryNum < count; categoryNum++)
			{
				var category = new Category
				{
					Id = Guid.NewGuid(),
					Name = $"{user.UserName}-Category{categoryNum}" + (areTheyDeleted ? "-Deleted" : string.Empty),
					OwnerId = user.Id,
					IsDeleted = areTheyDeleted
				};

				user.Categories.Add(category);
			}
		}

		private static void AddAccounts(
			ApplicationUser user,
			int count,
			IEnumerable<Guid> accountTypesIdsToBeUsed,
			IEnumerable<Guid> usedCurrenciesIdsToBeUsed,
			bool areTheyDeleted)
		{
			int accountTypeIndex = 0;
			int currencyIndex = 0;

			for (int accountNum = 0; accountNum < count; accountNum++)
			{
				if (accountTypesIdsToBeUsed.Count() == accountTypeIndex)
					accountTypeIndex = 0;

				if (usedCurrenciesIdsToBeUsed.Count() == currencyIndex)
					currencyIndex = 0;

				var account = new Account
				{
					Id = Guid.NewGuid(),
					Name = $"{user.UserName}-Account{accountNum}" + (areTheyDeleted ? "-Deleted" : string.Empty),
					AccountTypeId = accountTypesIdsToBeUsed.ElementAt(accountTypeIndex++),
					Balance = 0,
					CurrencyId = usedCurrenciesIdsToBeUsed.ElementAt(currencyIndex++),
					OwnerId = user.Id,
					IsDeleted = areTheyDeleted
				};

				user.Accounts.Add(account);
			}
		}

		private static void AddAccountsTransactions(
			IEnumerable<Account> accounts, 
			IEnumerable<Guid> categoriesIdsToBeUsed)
		{
			TransactionType initialBalanceType = TransactionType.Income;

			for (int i = 0; i < accounts.Count(); i++)
			{
				AddTransactions(
					accounts.ElementAt(i),
					count: 10,
					categoriesIdsToBeUsed,
					initialBalanceType);

				initialBalanceType = initialBalanceType == TransactionType.Income
					? TransactionType.Expense
					: TransactionType.Income;
			}
		}

		private static void AddTransactions(
			Account account,
			int count,
			IEnumerable<Guid> categoriesIdsToBeUsed,
			TransactionType initialBalanceType)
		{
			var randomizer = new Randomizer();
			int categoryIndex = 0;
			decimal lastTransactionAmount = 0;

			for (int transactionNum = 0; transactionNum < count; transactionNum++)
			{
				bool isInitialBalanceTransaction = transactionNum == 0;

				decimal amount = isInitialBalanceTransaction
					? initialBalanceType == TransactionType.Income
						? randomizer.NextDecimal(1000)
						: randomizer.NextDecimal(-1000, -1)
					: lastTransactionAmount >= 0
						? randomizer.NextDecimal(-1000, -1)
						: randomizer.NextDecimal(1000);

				lastTransactionAmount = amount;

				if (categoriesIdsToBeUsed.Count() == categoryIndex)
					categoryIndex = 0;

				int months = randomizer.Next(-3, 0);

				var transaction = new Transaction
				{
					Id = Guid.NewGuid(),
					OwnerId = account.OwnerId,
					AccountId = account.Id,
					Amount = amount,
					CategoryId = isInitialBalanceTransaction
						? Guid.Parse(InitialBalanceCategoryId)
						: categoriesIdsToBeUsed.ElementAt(categoryIndex++),
					CreatedOnUtc = DateTime.UtcNow.AddMonths(months),
					Reference = isInitialBalanceTransaction
						? CategoryInitialBalanceName
						: $"{account.Name}-Transaction{transactionNum}",
					TransactionType = amount < 0
						? TransactionType.Expense
						: TransactionType.Income,
					IsInitialBalance = isInitialBalanceTransaction
				};

				account.Transactions.Add(transaction);
			}

			CalculateAccountBalance(account);
		}

		private static void CalculateAccountBalance(Account account)
		{
			decimal expense = account.Transactions
				.Where(t => t.TransactionType == TransactionType.Expense)
				.Sum(t => t.Amount);

			decimal income = account.Transactions
				.Where(t => t.TransactionType == TransactionType.Income)
				.Sum(t => t.Amount);

			account.Balance = income - expense;
		}
	}
}
