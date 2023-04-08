namespace PersonalFinancer.Tests
{
	using AutoMapper;
	using Microsoft.Extensions.Caching.Memory;

	using NUnit.Framework;

	using Data;
	using Data.Enums;
	using Data.Models;
	using static Data.Constants.CategoryConstants;

	using Tests.Mocks;

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

		protected AccountType AccountType1 { get; private set; } = null!;
		protected AccountType AccountType2 { get; private set; } = null!;
		protected AccountType AccountType3 { get; private set; } = null!;

		protected Category Category1 { get; private set; } = null!;
		protected Category Category2 { get; private set; } = null!;
		protected Category Category3 { get; private set; } = null!;
		protected Category Category4 { get; private set; } = null!;

		protected Currency Currency1 { get; private set; } = null!;
		protected Currency Currency2 { get; private set; } = null!;

		protected Transaction Transaction1 { get; private set; } = null!;
		protected Transaction Transaction2 { get; private set; } = null!;
		protected Transaction Transaction3 { get; private set; } = null!;
		protected Transaction Transaction4 { get; private set; } = null!;
		protected Transaction Transaction5 { get; private set; } = null!;
		protected Transaction Transaction6 { get; private set; } = null!;

		private async void SeedDatabase()
		{
			//Users
			string PetarId = Guid.NewGuid().ToString();
			User1 = new ApplicationUser
			{
				Id = PetarId,
				FirstName = "Petar",
				LastName = "Petrov",
				Email = "petar@mail.com",
				UserName = "petar@mail.com",
			};
			data.Users.Add(User1);
			string TodorId = Guid.NewGuid().ToString();
			User2 = new ApplicationUser
			{
				Id = TodorId,
				FirstName = "Todor",
				LastName = "Todorov",
				Email = "todor@mail.com",
				UserName = "todor@mail.com",
			};
			data.Users.Add(User2);

			// Account Types
			string cashAccTypeId = Guid.NewGuid().ToString();
			AccountType1 = new AccountType
			{
				Id = cashAccTypeId,
				Name = "Cash",
				OwnerId = PetarId
			};
			string BankAccTypeId = Guid.NewGuid().ToString();
			AccountType2 = new AccountType
			{
				Id = BankAccTypeId,
				Name = "Bank",
				OwnerId = PetarId
			};
			string CustomAccTypeId = Guid.NewGuid().ToString();
			AccountType3 = new AccountType
			{
				Id = CustomAccTypeId,
				Name = "Bank",
				IsDeleted = true,
				OwnerId = PetarId
			};
			data.AccountTypes.AddRange(AccountType1, AccountType2, AccountType3);

			// Currencies
			string BgnCurrencyId = Guid.NewGuid().ToString();
			Currency1 = new Currency
			{
				Id = BgnCurrencyId,
				Name = "BGN",
				OwnerId = PetarId
			};
			string EurCurrencyId = Guid.NewGuid().ToString();
			Currency2 = new Currency
			{
				Id = EurCurrencyId,
				Name = "EUR",
				OwnerId = PetarId
			};
			//Guid UsdCurrencyId = Guid.NewGuid();
			//Currency3 = new Currency
			//{
			//	Id = UsdCurrencyId,
			//	Name = "USD"
			//};
			data.Currencies.AddRange(Currency1, Currency2/*, Currency3*/);

			// Accounts
			string petarCashBgnAccId = Guid.NewGuid().ToString();
			Account1User1 = new Account
			{
				Id = petarCashBgnAccId,
				Name = "Cash BGN",
				AccountTypeId = cashAccTypeId,
				Balance = 189.55m,
				CurrencyId = BgnCurrencyId,
				OwnerId = PetarId
			};
			string petarBankEurAccId = Guid.NewGuid().ToString();
			Account2User1 = new Account
			{
				Id = petarBankEurAccId,
				Name = "Bank EUR",
				AccountTypeId = BankAccTypeId,
				Balance = 900.01m,
				CurrencyId = EurCurrencyId,
				OwnerId = PetarId,
				IsDeleted = true
			};
			data.Accounts.AddRange(Account1User1, Account2User1);

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
				OwnerId = PetarId
			};
			string transportCatId = Guid.NewGuid().ToString();
			Category3 = new Category
			{
				Id = transportCatId,
				Name = "Transport",
				OwnerId = PetarId
			};
			string salaryCatId = Guid.NewGuid().ToString();
			Category4 = new Category
			{
				Id = salaryCatId,
				Name = "Salary",
				OwnerId = PetarId
			};
			data.Categories.AddRange(Category1, Category2, Category3, Category4);

			// Transactions
			// Cash BGN
			Transaction1 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = PetarId,
				AccountId = petarCashBgnAccId,
				Amount = 200,
				CategoryId = InitialBalanceCategoryId,
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Refference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income
			};
			Transaction2 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = PetarId,
				AccountId = petarCashBgnAccId,
				Amount = 5.65m,
				CategoryId = foodCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Lunch",
				TransactionType = TransactionType.Expense
			};
			Transaction3 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = PetarId,
				AccountId = petarCashBgnAccId,
				Amount = 4.80m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddDays(-2),
				Refference = "Taxi",
				TransactionType = TransactionType.Expense
			};
			// Bank EUR
			Transaction4 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = PetarId,
				AccountId = petarBankEurAccId,
				Amount = 200,
				CategoryId = InitialBalanceCategoryId,
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Refference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income
			};
			Transaction5 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = PetarId,
				AccountId = petarBankEurAccId,
				Amount = 750m,
				CategoryId = salaryCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			Transaction6 = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				OwnerId = PetarId,
				AccountId = petarBankEurAccId,
				Amount = 49.99m,
				CategoryId = transportCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Flight ticket",
				TransactionType = TransactionType.Expense
			};

			data.Transactions.AddRange(Transaction1, Transaction2, Transaction3, Transaction4, Transaction5, Transaction6);
			await data.SaveChangesAsync();
		}
	}
}
