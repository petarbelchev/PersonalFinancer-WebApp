namespace PersonalFinancer.Tests
{
	using NUnit.Framework;
	using AutoMapper;

	using Mocks;
	using Data;
	using Data.Models;
	using Data.Enums;
	using static Data.Constants.CategoryConstants;

	[TestFixture]
	abstract class UnitTestsBase
	{
		protected PersonalFinancerDbContext data;
		protected IMapper mapper;

		[OneTimeSetUp]
		protected void SetUpBase()
		{
			data = DatabaseMock.Instance;
			mapper = MapperMock.Instance;
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
		protected Category Category5 { get; private set; } = null!;
		protected Category Category6 { get; private set; } = null!;

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
			Guid cashAccTypeId = Guid.NewGuid();
			AccountType1 = new AccountType
			{
				Id = cashAccTypeId,
				Name = "Cash"
			};
			Guid BankAccTypeId = Guid.NewGuid();
			AccountType2 = new AccountType
			{
				Id = BankAccTypeId,
				Name = "Bank"
			};
			Guid CustomAccTypeId = Guid.NewGuid();
			AccountType3 = new AccountType
			{
				Id = CustomAccTypeId,
				Name = "Bank",
				IsDeleted = true
			};
			data.AccountTypes.AddRange(AccountType1, AccountType2, AccountType3);

			// Currencies
			Guid BgnCurrencyId = Guid.NewGuid();
			Currency1 = new Currency
			{
				Id = BgnCurrencyId,
				Name = "BGN"
			};
			Guid EurCurrencyId = Guid.NewGuid();
			Currency2 = new Currency
			{
				Id = EurCurrencyId,
				Name = "EUR"
			};
			//Guid UsdCurrencyId = Guid.NewGuid();
			//Currency3 = new Currency
			//{
			//	Id = UsdCurrencyId,
			//	Name = "USD"
			//};
			data.Currencies.AddRange(Currency1, Currency2/*, Currency3*/);

			// Accounts
			Guid petarCashBgnAccId = Guid.NewGuid();
			Account1User1 = new Account
			{
				Id = petarCashBgnAccId,
				Name = "Cash BGN",
				AccountTypeId = cashAccTypeId,
				Balance = 189.55m,
				CurrencyId = BgnCurrencyId,
				OwnerId = PetarId
			};
			Guid petarBankEurAccId = Guid.NewGuid();
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
			Guid initBalanceCatId = Guid.NewGuid();
			Category1 = new Category
			{
				Id = initBalanceCatId,
				Name = CategoryInitialBalanceName,
			};
			Guid foodCatId = Guid.NewGuid();
			Category2 = new Category
			{
				Id = foodCatId,
				Name = "Food and Drinks"
			};
			Guid transportCatId = Guid.NewGuid();
			Category3 = new Category
			{
				Id = transportCatId,
				Name = "Transport"
			};
			Guid salaryCatId = Guid.NewGuid();
			Category4 = new Category
			{
				Id = salaryCatId,
				Name = "Salary"
			};
			Guid customCatId = Guid.NewGuid();
			Category5 = new Category
			{
				Id = customCatId,
				Name = "Custom Category",
				UserId = PetarId
			};
			Guid deletedCustomCatId = Guid.NewGuid();
			Category6 = new Category
			{
				Id = deletedCustomCatId,
				Name = "Custom Category",
				UserId = PetarId,
				IsDeleted = true
			};
			data.Categories.AddRange(Category1, Category2, Category3, Category4, Category5, Category6);

			// Transactions
			// Cash BGN
			Transaction1 = new Transaction()
			{
				Id = Guid.NewGuid(),
				AccountId = petarCashBgnAccId,
				Amount = 200,
				CategoryId = initBalanceCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Refference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income
			};
			Transaction2 = new Transaction()
			{
				Id = Guid.NewGuid(),
				AccountId = petarCashBgnAccId,
				Amount = 5.65m,
				CategoryId = foodCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Lunch",
				TransactionType = TransactionType.Expense
			};
			Transaction3 = new Transaction()
			{
				Id = Guid.NewGuid(),
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
				Id = Guid.NewGuid(),
				AccountId = petarBankEurAccId,
				Amount = 200,
				CategoryId = initBalanceCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-3),
				Refference = CategoryInitialBalanceName,
				TransactionType = TransactionType.Income
			};
			Transaction5 = new Transaction()
			{
				Id = Guid.NewGuid(),
				AccountId = petarBankEurAccId,
				Amount = 750m,
				CategoryId = salaryCatId,
				CreatedOn = DateTime.UtcNow.AddMonths(-2),
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			Transaction6 = new Transaction()
			{
				Id = Guid.NewGuid(),
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
