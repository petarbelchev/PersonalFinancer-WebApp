using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Tests.Mocks;
using static PersonalFinancer.Data.Constants.TransactionConstants;

namespace PersonalFinancer.Tests
{
	[TestFixture]
	abstract class UnitTestsBase
	{
		protected PersonalFinancerDbContext data;
		protected IMapper mapper;
		protected IMemoryCache memoryCache;

		[OneTimeSetUp]
		protected void SetUpBase()
		{
			data = DatabaseMock.Instance;
			mapper = MapperMock.Instance;
			memoryCache = MemoryCacheMock.Instance;

			SeedDatabase();
		}

		[OneTimeTearDown]
		protected void TearDownBase()
		{
			data.Dispose();
		}

		protected ApplicationUser User1 { get; private set; } = null!;
		protected ApplicationUser User2 { get; private set; } = null!;

		protected Account Account1User1 { get; private set; } = null!;
		protected Account Account2User1 { get; private set; } = null!;
		protected Account Account2User2 { get; private set; } = null!;

		protected AccountType AccountType1User1 { get; private set; } = null!;
		protected AccountType AccountType2User1 { get; private set; } = null!;
		protected AccountType AccountType3User2 { get; private set; } = null!;

		protected Category Category1 { get; private set; } = null!;
		protected Category Category2 { get; private set; } = null!;
		protected Category Category3 { get; private set; } = null!;
		protected Category Category4 { get; private set; } = null!;

		protected Currency Currency1User1 { get; private set; } = null!;
		protected Currency Currency2User1 { get; private set; } = null!;
		protected Currency Currency3User2 { get; private set; } = null!;

		protected Transaction Transaction1User1 { get; private set; } = null!;
		protected Transaction Transaction2User1 { get; private set; } = null!;
		protected Transaction Transaction3User1 { get; private set; } = null!;
		protected Transaction Transaction4User1 { get; private set; } = null!;
		protected Transaction Transaction5User1 { get; private set; } = null!;
		protected Transaction Transaction6User1 { get; private set; } = null!;

		private async void SeedDatabase()
		{
			//Users
			string User1Id = Guid.NewGuid().ToString();
			User1 = new ApplicationUser
			{
				Id = User1Id,
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				UserName = "petar@mail.com",
			};
			data.Users.Add(User1);
			string User2Id = Guid.NewGuid().ToString();
			User2 = new ApplicationUser
			{
				Id = User2Id,
				FirstName = "Todor",
				LastName = "Todorov",
				Email = "todor@mail.com",
				UserName = "todor@mail.com",
			};
			data.Users.Add(User2);

			// Account Types
			string cashAccTypeId = Guid.NewGuid().ToString();
			AccountType1User1 = new AccountType
			{
				Id = cashAccTypeId,
				Name = "Cash",
				OwnerId = User1Id
			};
			string BankAccTypeId = Guid.NewGuid().ToString();
			AccountType2User1 = new AccountType
			{
				Id = BankAccTypeId,
				Name = "Bank",
				OwnerId = User1Id
			};
			string User2AccTypeId = Guid.NewGuid().ToString();
			AccountType3User2 = new AccountType
			{
				Id = User2AccTypeId,
				Name = "Bank",
				OwnerId = User2Id
			};
			data.AccountTypes.AddRange(AccountType1User1, AccountType2User1, AccountType3User2);

			// Currencies
			string BgnCurrencyId = Guid.NewGuid().ToString();
			Currency1User1 = new Currency
			{
				Id = BgnCurrencyId,
				Name = "BGN",
				OwnerId = User1Id
			};
			string EurCurrencyId = Guid.NewGuid().ToString();
			Currency2User1 = new Currency
			{
				Id = EurCurrencyId,
				Name = "EUR",
				OwnerId = User1Id
			};
			string User2UsdCurrencyId = Guid.NewGuid().ToString();
			Currency3User2 = new Currency
			{
				Id = User2UsdCurrencyId,
				Name = "USD",
				OwnerId = User2Id
			};
			data.Currencies.AddRange(Currency1User1, Currency2User1, Currency3User2);

			// Accounts
			string user1CashBgnAccId = Guid.NewGuid().ToString();
			Account1User1 = new Account
			{
				Id = user1CashBgnAccId,
				Name = "Cash BGN",
				AccountTypeId = cashAccTypeId,
				Balance = 189.55m,
				CurrencyId = BgnCurrencyId,
				OwnerId = User1Id
			};
			string user1BankEurAccId = Guid.NewGuid().ToString();
			Account2User1 = new Account
			{
				Id = user1BankEurAccId,
				Name = "Bank EUR",
				AccountTypeId = BankAccTypeId,
				Balance = 900.01m,
				CurrencyId = EurCurrencyId,
				OwnerId = User1Id,
				IsDeleted = true
			};
			string user2BankEurAccId = Guid.NewGuid().ToString();
			Account2User2 = new Account
			{
				Id = user2BankEurAccId,
				Name = "Bank USD",
				AccountTypeId = User2AccTypeId,
				Balance = 0,
				CurrencyId = User2UsdCurrencyId,
				OwnerId = User1Id,
				IsDeleted = true
			};
			data.Accounts.AddRange(Account1User1, Account2User1, Account2User2);

			// Categories
			Category1 = new Category
			{
				Id = InitialBalanceCategoryId,
				Name = CategoryInitialBalanceName,
				OwnerId = "adminId"
			};
			string foodCatId = Guid.NewGuid().ToString();
			Category2 = new Category
			{
				Id = foodCatId,
				Name = "Food and Drinks",
				OwnerId = User1Id
			};
			string transportCatId = Guid.NewGuid().ToString();
			Category3 = new Category
			{
				Id = transportCatId,
				Name = "Transport",
				OwnerId = User1Id
			};
			string salaryCatId = Guid.NewGuid().ToString();
			Category4 = new Category
			{
				Id = salaryCatId,
				Name = "Salary",
				OwnerId = User1Id
			};
			data.Categories.AddRange(Category1, Category2, Category3, Category4);

			// Transactions
			// Cash BGN
			Transaction1User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = User1Id,
				AccountId = user1CashBgnAccId,
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
				OwnerId = User1Id,
				AccountId = user1CashBgnAccId,
				Amount = 5.65m,
				CategoryId = foodCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Lunch",
				TransactionType = TransactionType.Expense
			};
			Transaction3User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = User1Id,
				AccountId = user1CashBgnAccId,
				Amount = 4.80m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddDays(-2),
				Refference = "Taxi",
				TransactionType = TransactionType.Expense
			};
			// Bank EUR
			Transaction4User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = User1Id,
				AccountId = user1BankEurAccId,
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
				OwnerId = User1Id,
				AccountId = user1BankEurAccId,
				Amount = 750m,
				CategoryId = salaryCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			Transaction6User1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = User1Id,
				AccountId = user1BankEurAccId,
				Amount = 49.99m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Flight ticket",
				TransactionType = TransactionType.Expense
			};

			data.Transactions.AddRange(Transaction1User1, Transaction2User1, Transaction3User1, Transaction4User1, Transaction5User1, Transaction6User1);
			await data.SaveChangesAsync();
		}
	}
}
