namespace PersonalFinancer.Tests.Services
{
	using AutoMapper;
	using Microsoft.Extensions.Caching.Memory;
	using NUnit.Framework;
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Tests.Mocks;
	using static PersonalFinancer.Data.Constants.CategoryConstants;

	[TestFixture]
	internal abstract class ServicesUnitTestsBase
	{
		protected PersonalFinancerDbContext sqlDbContext;
		protected IMapper mapper;
		protected IMemoryCache memoryCache;

		[OneTimeSetUp]
		protected async Task SetUpBase()
		{
			this.sqlDbContext = DatabaseMock.Instance;
			this.mapper = ServicesMapperMock.Instance;
			this.memoryCache = MemoryCacheMock.Instance;

			await this.SeedDatabase();
		}

		[OneTimeTearDown]
		protected async Task TearDownBase() => await this.sqlDbContext.DisposeAsync();

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
			var user1Id = Guid.NewGuid();
			var user2Id = Guid.NewGuid();
			this.User1 = new ApplicationUser
			{
				Id = user1Id,
				UserName = "petar",
				NormalizedUserName = "PETAR",
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				NormalizedEmail = "PETAR@MAIL.COM"
			};
			this.User2 = new ApplicationUser
			{
				Id = user2Id,
				UserName = "todor",
				NormalizedUserName = "TODOR",
				FirstName = "Todor",
				LastName = "Todorov",
				Email = "todor@mail.com",
				NormalizedEmail = "TODOR@MAIL.COM"
			};

			await this.sqlDbContext.Users.AddRangeAsync(this.User1, this.User2);

			// Account Types
			var accType1Id = Guid.NewGuid();
			var accType2Id = Guid.NewGuid();
			var accType3Id = Guid.NewGuid();
			this.AccType1User1 = new AccountType
			{
				Id = accType1Id,
				Name = "Cash",
				OwnerId = user1Id
			};
			this.AccType2User1 = new AccountType
			{
				Id = accType2Id,
				Name = "Bank",
				OwnerId = user1Id
			};
			this.AccType3User1 = new AccountType
			{
				Id = accType3Id,
				Name = "Bank",
				OwnerId = user1Id
			};

			await this.sqlDbContext.AccountTypes.AddRangeAsync(this.AccType1User1, this.AccType2User1, this.AccType3User1);

			// Currencies
			var curr1Id = Guid.NewGuid();
			var EurCurrencyId = Guid.NewGuid();
			var User2UsdCurrencyId = Guid.NewGuid();
			this.Curr1User1 = new Currency
			{
				Id = curr1Id,
				Name = "BGN",
				OwnerId = user1Id
			};
			this.Curr2User1 = new Currency
			{
				Id = EurCurrencyId,
				Name = "EUR",
				OwnerId = user1Id
			};
			this.Curr3User1 = new Currency
			{
				Id = User2UsdCurrencyId,
				Name = "USD",
				OwnerId = user1Id
			};

			await this.sqlDbContext.Currencies.AddRangeAsync(this.Curr1User1, this.Curr2User1, this.Curr3User1);

			// Accounts
			var acc1Id = Guid.NewGuid();
			var acc2Id = Guid.NewGuid();
			var acc3Id = Guid.NewGuid();
			this.Account1User1 = new Account
			{
				Id = acc1Id,
				Name = "Cash BGN",
				AccountTypeId = accType1Id,
				Balance = 189.55m,
				CurrencyId = curr1Id,
				OwnerId = user1Id
			};
			this.Account2User1 = new Account
			{
				Id = acc2Id,
				Name = "Bank EUR",
				AccountTypeId = accType1Id,
				Balance = 900.01m,
				CurrencyId = curr1Id,
				OwnerId = user1Id
			};
			this.Account3User1Deleted = new Account
			{
				Id = acc3Id,
				Name = "Bank USD",
				AccountTypeId = acc1Id,
				Balance = 0,
				CurrencyId = curr1Id,
				OwnerId = user1Id,
				IsDeleted = true
			};

			await this.sqlDbContext.Accounts.AddRangeAsync(this.Account1User1, this.Account2User1, this.Account3User1Deleted);

			// Categories
			var foodCatId = Guid.NewGuid();
			var transportCatId = Guid.NewGuid();
			var salaryCatId = Guid.NewGuid();
			this.CatInitialBalance = new Category
			{
				Id = Guid.Parse(InitialBalanceCategoryId),
				Name = CategoryInitialBalanceName,
				OwnerId = Guid.NewGuid()
			};
			this.Cat2User1 = new Category
			{
				Id = foodCatId,
				Name = "Food and Drinks",
				OwnerId = user1Id
			};
			this.Cat3User1 = new Category
			{
				Id = transportCatId,
				Name = "Transport",
				OwnerId = user1Id
			};
			this.Cat4User1 = new Category
			{
				Id = salaryCatId,
				Name = "Salary",
				OwnerId = user1Id
			};

			await this.sqlDbContext.Categories.AddRangeAsync(this.CatInitialBalance, this.Cat2User1, this.Cat3User1, this.Cat4User1);

			// Transactions
			// Account 1
			this.Transaction1User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = user1Id,
				AccountId = acc1Id,
				Amount = 200,
				CategoryId = Guid.Parse(InitialBalanceCategoryId),
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Reference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income,
				IsInitialBalance = true
			};
			this.Transaction2User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = user1Id,
				AccountId = acc1Id,
				Amount = 5.65m,
				CategoryId = this.Cat2User1.Id,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Reference = "Lunch",
				TransactionType = TransactionType.Expense
			};
			this.Transaction3User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = user1Id,
				AccountId = acc1Id,
				Amount = 4.80m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddDays(-2),
				Reference = "Taxi",
				TransactionType = TransactionType.Expense
			};
			// Account 2
			this.Transaction4User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = user1Id,
				AccountId = acc2Id,
				Amount = 200,
				CategoryId = Guid.Parse(InitialBalanceCategoryId),
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Reference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income,
				IsInitialBalance = true
			};
			this.Transaction5User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = user1Id,
				AccountId = acc2Id,
				Amount = 750m,
				CategoryId = salaryCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Reference = "Salary",
				TransactionType = TransactionType.Income
			};
			this.Transaction6User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = user1Id,
				AccountId = acc2Id,
				Amount = 49.99m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Reference = "Flight ticket",
				TransactionType = TransactionType.Expense
			};

			await this.sqlDbContext.Transactions.AddRangeAsync(
				this.Transaction1User1, this.Transaction2User1, this.Transaction3User1,
				this.Transaction4User1, this.Transaction5User1, this.Transaction6User1);

			await this.sqlDbContext.SaveChangesAsync();
		}
	}
}
