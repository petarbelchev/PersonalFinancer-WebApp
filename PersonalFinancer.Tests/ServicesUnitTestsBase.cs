using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using NUnit.Framework;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using static PersonalFinancer.Data.Constants.CategoryConstants;

using PersonalFinancer.Tests.Mocks;

namespace PersonalFinancer.Tests
{
	[TestFixture]
	abstract class ServicesUnitTestsBase
	{
		protected SqlDbContext sqlDbContext;
		protected IMapper mapper;
		protected IMemoryCache memoryCache;

		[OneTimeSetUp]
		protected async Task SetUpBase()
		{
			sqlDbContext = DatabaseMock.Instance;
			mapper = ServicesMapperMock.Instance;
			memoryCache = MemoryCacheMock.Instance;

			await SeedDatabase();
		}

		[OneTimeTearDown]
		protected async Task TearDownBase()
		{
			await sqlDbContext.DisposeAsync();
		}

		protected ApplicationUser User1 { get; private set; } = null!;
		protected ApplicationUser User2 { get; private set; } = null!;

		protected Account Account1User1 { get; private set; } = null!;
		protected Account Account2User1 { get; private set; } = null!;
		protected Account Account3User1Deleted { get; private set; } = null!;

		protected AccountType AccType1User1 { get; private set; } = null!;
		protected AccountType AccType2User1 { get; private set; } = null!;
		protected AccountType AccType3User1 { get; private set; } = null!;

		protected Category CatInitialBalance { get; private set; } = null!;
		protected Category Cat2User1 { get; private set; } = null!;
		protected Category Cat3User1 { get; private set; } = null!;
		protected Category Cat4User1 { get; private set; } = null!;

		protected Currency Curr1User1 { get; private set; } = null!;
		protected Currency Curr2User1 { get; private set; } = null!;
		protected Currency Curr3User1 { get; private set; } = null!;

		protected Transaction Transaction1User1 { get; private set; } = null!;
		protected Transaction Transaction2User1 { get; private set; } = null!;
		protected Transaction Transaction3User1 { get; private set; } = null!;
		protected Transaction Transaction4User1 { get; private set; } = null!;
		protected Transaction Transaction5User1 { get; private set; } = null!;
		protected Transaction Transaction6User1 { get; private set; } = null!;

		private async Task SeedDatabase()
		{
			//Users
			string user1Id = Guid.NewGuid().ToString();
			string user2Id = Guid.NewGuid().ToString();
			User1 = new ApplicationUser
			{
				Id = user1Id,
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				UserName = "petar@mail.com",
			};
			User2 = new ApplicationUser
			{
				Id = user2Id,
				FirstName = "Todor",
				LastName = "Todorov",
				Email = "todor@mail.com",
				UserName = "todor@mail.com",
			};

			await sqlDbContext.Users.AddRangeAsync(User1, User2);

			// Account Types
			string accType1Id = Guid.NewGuid().ToString();
			string accType2Id = Guid.NewGuid().ToString();
			string accType3Id = Guid.NewGuid().ToString();
			AccType1User1 = new AccountType
			{
				Id = accType1Id,
				Name = "Cash",
				OwnerId = user1Id
			};
			AccType2User1 = new AccountType
			{
				Id = accType2Id,
				Name = "Bank",
				OwnerId = user1Id
			};
			AccType3User1 = new AccountType
			{
				Id = accType3Id,
				Name = "Bank",
				OwnerId = user1Id
			};

			await sqlDbContext.AccountTypes.AddRangeAsync(AccType1User1, AccType2User1, AccType3User1);

			// Currencies
			string curr1Id = Guid.NewGuid().ToString();
			string EurCurrencyId = Guid.NewGuid().ToString();
			string User2UsdCurrencyId = Guid.NewGuid().ToString();
			Curr1User1 = new Currency
			{
				Id = curr1Id,
				Name = "BGN",
				OwnerId = user1Id
			};
			Curr2User1 = new Currency
			{
				Id = EurCurrencyId,
				Name = "EUR",
				OwnerId = user1Id
			};
			Curr3User1 = new Currency
			{
				Id = User2UsdCurrencyId,
				Name = "USD",
				OwnerId = user1Id
			};

			await sqlDbContext.Currencies.AddRangeAsync(Curr1User1, Curr2User1, Curr3User1);

			//// Accounts
			string acc1Id = Guid.NewGuid().ToString();
			string acc2Id = Guid.NewGuid().ToString();
			string acc3Id = Guid.NewGuid().ToString();
			Account1User1 = new Account
			{
				Id = acc1Id,
				Name = "Cash BGN",
				AccountTypeId = accType1Id,
				Balance = 189.55m,
				CurrencyId = curr1Id,
				OwnerId = user1Id
			};
			Account2User1 = new Account
			{
				Id = acc2Id,
				Name = "Bank EUR",
				AccountTypeId = accType1Id,
				Balance = 900.01m,
				CurrencyId = curr1Id,
				OwnerId = user1Id
			};
			Account3User1Deleted = new Account
			{
				Id = acc3Id,
				Name = "Bank USD",
				AccountTypeId = acc1Id,
				Balance = 0,
				CurrencyId = curr1Id,
				OwnerId = user1Id,
				IsDeleted = true
			};

			await sqlDbContext.Accounts.AddRangeAsync(Account1User1, Account2User1, Account3User1Deleted);

			// Categories
			string foodCatId = Guid.NewGuid().ToString();
			string transportCatId = Guid.NewGuid().ToString();
			string salaryCatId = Guid.NewGuid().ToString();
			CatInitialBalance = new Category
			{
				Id = InitialBalanceCategoryId,
				Name = CategoryInitialBalanceName,
				OwnerId = "adminId"
			};
			Cat2User1 = new Category
			{
				Id = foodCatId,
				Name = "Food and Drinks",
				OwnerId = user1Id
			};
			Cat3User1 = new Category
			{
				Id = transportCatId,
				Name = "Transport",
				OwnerId = user1Id
			};
			Cat4User1 = new Category
			{
				Id = salaryCatId,
				Name = "Salary",
				OwnerId = user1Id
			};

			await sqlDbContext.Categories.AddRangeAsync(CatInitialBalance, Cat2User1, Cat3User1, Cat4User1);

			// Transactions
			// Account 1
			Transaction1User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = user1Id,
				AccountId = acc1Id,
				Amount = 200,
				CategoryId = InitialBalanceCategoryId,
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Refference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income,
				IsInitialBalance = true
			};
			Transaction2User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = user1Id,
				AccountId = acc1Id,
				Amount = 5.65m,
				CategoryId = Cat2User1.Id,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Lunch",
				TransactionType = TransactionType.Expense
			};
			Transaction3User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = user1Id,
				AccountId = acc1Id,
				Amount = 4.80m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddDays(-2),
				Refference = "Taxi",
				TransactionType = TransactionType.Expense
			};
			// Account 2
			Transaction4User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = user1Id,
				AccountId = acc2Id,
				Amount = 200,
				CategoryId = InitialBalanceCategoryId,
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Refference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income,
				IsInitialBalance = true
			};
			Transaction5User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = user1Id,
				AccountId = acc2Id,
				Amount = 750m,
				CategoryId = salaryCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			Transaction6User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = user1Id,
				AccountId = acc2Id,
				Amount = 49.99m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Flight ticket",
				TransactionType = TransactionType.Expense
			};

			await sqlDbContext.Transactions.AddRangeAsync(
				Transaction1User1, Transaction2User1, Transaction3User1, 
				Transaction4User1, Transaction5User1, Transaction6User1);

			await sqlDbContext.SaveChangesAsync();
		}
	}
}
