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

		[SetUp]
		protected async Task SetUpBase()
		{
			this.sqlDbContext = DatabaseMock.Instance;
			this.mapper = ServicesMapperMock.Instance;
			this.memoryCache = MemoryCacheMock.Instance;

			await this.SeedDatabase();
		}

		[TearDown]
		protected async Task TearDownBase() => await this.sqlDbContext.DisposeAsync();

		protected ApplicationUser User1 { get; private set; } = null!;
		protected ApplicationUser User2 { get; private set; } = null!;

		protected Account Account1_User1_WithTransactions { get; private set; } = null!;
		protected Account Account2_User1_WithoutTransactions { get; private set; } = null!;
		protected Account Account3_User1_Deleted_WithTransactions { get; private set; } = null!;
		protected Account Account4_User1_Deleted_WithoutTransactions { get; private set; } = null!;

		protected AccountType AccType1_User1_WithAcc { get; private set; } = null!;
		protected AccountType AccType2_User1_WithoutAcc { get; private set; } = null!;
		protected AccountType AccType3_User1_Deleted_WithAcc { get; private set; } = null!;
		protected AccountType AccType4_User1_Deleted_WithoutAcc { get; private set; } = null!;

		protected Category Category_InitialBalance { get; private set; } = null!;
		protected Category Category1_User1_WithTransactions { get; private set; } = null!;
		protected Category Category2_User1_WithoutTransactions { get; private set; } = null!;
		protected Category Category3_User1_Deleted_WithTransactions { get; private set; } = null!;
		protected Category Category4_User1_Deleted_WithoutTransactions { get; private set; } = null!;

		protected Currency Currency1_User1_WithAcc { get; private set; } = null!;
		protected Currency Currency2_User1_WithoutAcc { get; private set; } = null!;
		protected Currency Currency3_User1_Deleted_WithAcc { get; private set; } = null!;
		protected Currency Currency4_User1_Deleted_WithoutAcc { get; private set; } = null!;

		protected Transaction Transaction1_User1 { get; private set; } = null!;
		protected Transaction Transaction2_User1 { get; private set; } = null!;
		protected Transaction Transaction3_User1 { get; private set; } = null!;
		protected Transaction Transaction4_User1 { get; private set; } = null!;
		protected Transaction Transaction5_User1 { get; private set; } = null!;
		protected Transaction Transaction6_User1 { get; private set; } = null!;

		protected async Task SeedDatabase()
		{
			//Users
			this.User1 = new ApplicationUser
			{
				Id = Guid.NewGuid(),
				UserName = "petar",
				NormalizedUserName = "PETAR",
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				NormalizedEmail = "PETAR@MAIL.COM"
			};
			this.User2 = new ApplicationUser
			{
				Id = Guid.NewGuid(),
				UserName = "todor",
				NormalizedUserName = "TODOR",
				FirstName = "Todor",
				LastName = "Todorov",
				Email = "todor@mail.com",
				NormalizedEmail = "TODOR@MAIL.COM"
			};
			await this.sqlDbContext.Users.AddRangeAsync(
				this.User1,
				this.User2);

			// Account Types
			this.AccType1_User1_WithAcc = new AccountType
			{
				Id = Guid.NewGuid(),
				Name = "Cash",
				OwnerId = this.User1.Id,
				IsDeleted = false
			};
			this.AccType2_User1_WithoutAcc = new AccountType
			{
				Id = Guid.NewGuid(),
				Name = "Bank",
				OwnerId = this.User1.Id,
				IsDeleted = false
			};
			this.AccType3_User1_Deleted_WithAcc = new AccountType
			{
				Id = Guid.NewGuid(),
				Name = "Credit",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			this.AccType4_User1_Deleted_WithoutAcc = new AccountType
			{
				Id = Guid.NewGuid(),
				Name = "Crypto",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await this.sqlDbContext.AccountTypes.AddRangeAsync(
				this.AccType1_User1_WithAcc,
				this.AccType2_User1_WithoutAcc,
				this.AccType3_User1_Deleted_WithAcc,
				this.AccType4_User1_Deleted_WithoutAcc);

			// Currencies
			this.Currency1_User1_WithAcc = new Currency
			{
				Id = Guid.NewGuid(),
				Name = "BGN",
				OwnerId = this.User1.Id,
				IsDeleted = false
			};
			this.Currency2_User1_WithoutAcc = new Currency
			{
				Id = Guid.NewGuid(),
				Name = "EUR",
				OwnerId = this.User1.Id,
				IsDeleted = false
			};
			this.Currency3_User1_Deleted_WithAcc = new Currency
			{
				Id = Guid.NewGuid(),
				Name = "USD",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			this.Currency4_User1_Deleted_WithoutAcc = new Currency
			{
				Id = Guid.NewGuid(),
				Name = "SEK",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await this.sqlDbContext.Currencies.AddRangeAsync(
				this.Currency1_User1_WithAcc,
				this.Currency2_User1_WithoutAcc,
				this.Currency3_User1_Deleted_WithAcc,
				this.Currency4_User1_Deleted_WithoutAcc);

			// Accounts
			this.Account1_User1_WithTransactions = new Account
			{
				Id = Guid.NewGuid(),
				Name = "Cash BGN",
				AccountTypeId = this.AccType1_User1_WithAcc.Id,
				Balance = 189.55m,
				CurrencyId = this.Currency1_User1_WithAcc.Id,
				OwnerId = this.User1.Id,
				IsDeleted = false
			};
			this.Account2_User1_WithoutTransactions = new Account
			{
				Id = Guid.NewGuid(),
				Name = "Bank EUR",
				AccountTypeId = this.AccType3_User1_Deleted_WithAcc.Id,
				Balance = 900.01m,
				CurrencyId = this.Currency3_User1_Deleted_WithAcc.Id,
				OwnerId = this.User1.Id,
				IsDeleted = false
			};
			this.Account3_User1_Deleted_WithTransactions = new Account
			{
				Id = Guid.NewGuid(),
				Name = "Bank USD",
				AccountTypeId = this.Account1_User1_WithTransactions.Id,
				Balance = 0,
				CurrencyId = this.Currency1_User1_WithAcc.Id,
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			this.Account4_User1_Deleted_WithoutTransactions = new Account
			{
				Id = Guid.NewGuid(),
				Name = "Bank SEK",
				AccountTypeId = this.Account1_User1_WithTransactions.Id,
				Balance = 0,
				CurrencyId = this.Currency1_User1_WithAcc.Id,
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await this.sqlDbContext.Accounts.AddRangeAsync(
				this.Account1_User1_WithTransactions, 
				this.Account2_User1_WithoutTransactions, 
				this.Account3_User1_Deleted_WithTransactions,
				this.Account4_User1_Deleted_WithoutTransactions);

			// Categories
			this.Category_InitialBalance = new Category
			{
				Id = Guid.Parse(InitialBalanceCategoryId),
				Name = CategoryInitialBalanceName,
				OwnerId = Guid.NewGuid()
			};
			this.Category1_User1_WithTransactions = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Food and Drinks",
				OwnerId = this.User1.Id
			};
			this.Category2_User1_WithoutTransactions = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Transport",
				OwnerId = this.User1.Id
			};
			this.Category3_User1_Deleted_WithTransactions = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Salary",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			this.Category4_User1_Deleted_WithoutTransactions = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Utilities",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await this.sqlDbContext.Categories.AddRangeAsync(
				this.Category_InitialBalance, 
				this.Category1_User1_WithTransactions, 
				this.Category2_User1_WithoutTransactions, 
				this.Category3_User1_Deleted_WithTransactions,
				this.Category4_User1_Deleted_WithoutTransactions);

			// Transactions
			this.Transaction1_User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = this.User1.Id,
				AccountId = this.Account1_User1_WithTransactions.Id,
				Amount = 200,
				CategoryId = Guid.Parse(InitialBalanceCategoryId),
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Reference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income,
				IsInitialBalance = true
			};
			this.Transaction2_User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = this.User1.Id,
				AccountId = this.Account1_User1_WithTransactions.Id,
				Amount = 5.65m,
				CategoryId = this.Category1_User1_WithTransactions.Id,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Reference = "Lunch",
				TransactionType = TransactionType.Expense
			};
			this.Transaction3_User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = this.User1.Id,
				AccountId = this.Account1_User1_WithTransactions.Id,
				Amount = 4.80m,
				CategoryId = this.Category3_User1_Deleted_WithTransactions.Id,
				CreatedOn = DateTime.UtcNow.AddDays(-2),
				Reference = "Taxi",
				TransactionType = TransactionType.Expense
			};
			this.Transaction4_User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = this.User1.Id,
				AccountId = this.Account3_User1_Deleted_WithTransactions.Id,
				Amount = 200,
				CategoryId = Guid.Parse(InitialBalanceCategoryId),
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Reference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income,
				IsInitialBalance = true
			};
			this.Transaction5_User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = this.User1.Id,
				AccountId = this.Account3_User1_Deleted_WithTransactions.Id,
				Amount = 750m,
				CategoryId = this.Category3_User1_Deleted_WithTransactions.Id,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Reference = "Salary",
				TransactionType = TransactionType.Income
			};
			this.Transaction6_User1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				OwnerId = this.User1.Id,
				AccountId = this.Account3_User1_Deleted_WithTransactions.Id,
				Amount = 49.99m,
				CategoryId = this.Category3_User1_Deleted_WithTransactions.Id,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Reference = "Flight ticket",
				TransactionType = TransactionType.Expense
			};
			await this.sqlDbContext.Transactions.AddRangeAsync(
				this.Transaction1_User1, 
				this.Transaction2_User1, 
				this.Transaction3_User1,
				this.Transaction4_User1, 
				this.Transaction5_User1, 
				this.Transaction6_User1);

			await this.sqlDbContext.SaveChangesAsync();
		}
	}
}
