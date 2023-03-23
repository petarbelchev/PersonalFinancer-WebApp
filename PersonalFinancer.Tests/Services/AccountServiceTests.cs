using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	class AccountServiceTests : UnitTestsBase
	{
		private IAccountService accountService;

		[SetUp]
		public void SetUp()
		{
			this.accountService = new AccountService(this.data, this.mapper, this.memoryCache);
		}

		//[Test]
		//public async Task AllAccountsDropdownViewModel_ShouldReturnCorrectData_WithValidUserId()
		//{
		//	//Arrange
		//	IEnumerable<AccountDropdownViewModel> expectedAccounts = data.Accounts
		//		.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
		//		.Select(a => mapper.Map<AccountDropdownViewModel>(a))
		//		.AsEnumerable();

		//	//Act
		//	IEnumerable<AccountDropdownViewModel> actualAccounts =
		//		await accountService.GetUserAccountsDropdownViewModel(this.User1.Id);

		//	//Assert
		//	Assert.That(actualAccounts, Is.Not.Null);
		//	Assert.That(actualAccounts.Count(), Is.EqualTo(expectedAccounts.Count()));

		//	for (int i = 0; i < actualAccounts.Count(); i++)
		//	{
		//		Assert.That(actualAccounts.ElementAt(i).Id,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Id));
		//		Assert.That(actualAccounts.ElementAt(i).Name,
		//			Is.EqualTo(expectedAccounts.ElementAt(i).Name));
		//	}
		//}

		[Test]
		public async Task AccountDropdownViewModel_ShouldReturnAccount_WithValidId()
		{
			//Act
			AccountDropdownViewModel? actualAccount = await accountService
				.GetAccountDropdownViewModel(this.Account1User1.Id);

			//Assert
			Assert.That(actualAccount, Is.Not.Null);
			Assert.That(actualAccount.Id, Is.EqualTo(this.Account1User1.Id));
			Assert.That(actualAccount.Name, Is.EqualTo(this.Account1User1.Name));
		}

		[Test]
		public void AccountDropdownViewModel_ShouldReturnNull_WithInvalidId()
		{
			//Act & Assert
			Assert.That(async () => await accountService.GetAccountDropdownViewModel(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task AccountDetailsViewModel_ShouldReturnAccountDetails_WithValidId()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;
			int page = 1;

			DetailsAccountViewModel? expected = await data.Accounts
				.Where(a => a.Id == this.Account1User1.Id && !a.IsDeleted)
				.Select(a => new DetailsAccountViewModel
				{
					Name = a.Name,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name,
					StartDate = startDate,
					EndDate = endDate,
					Pagination = new PaginationModel
					{
						Page = page,
						TotalElements = a.Transactions.Count(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
					},
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Skip(page != 1 ? 10 * (page - 1) : 0)
						.Take(10)
						.Select(t => new TransactionTableViewModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn,
							CategoryName = t.Category.Name,
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference
						})
						.AsEnumerable()
				})
				.FirstOrDefaultAsync();

			//Aseert
			Assert.That(expected, Is.Not.Null);

			//Act
			DetailsAccountViewModel? actual = await accountService
				.GetAccountDetailsViewModel(this.Account1User1.Id, startDate, endDate);

			//Assert
			Assert.That(actual, Is.Not.Null);
			//Assert.That(actual.Dates.Id, Is.EqualTo(expected.Dates.Id));
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
			Assert.That(actual.Balance, Is.EqualTo(expected.Balance));
			Assert.That(actual.Transactions.Count(), Is.EqualTo(expected.Transactions.Count()));

			for (int i = 0; i < expected.Transactions.Count(); i++)
			{
				Assert.That(actual.Transactions.ElementAt(i).Id,
					Is.EqualTo(expected.Transactions.ElementAt(i).Id));
				Assert.That(actual.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expected.Transactions.ElementAt(i).Amount));
			}
		}

		[Test]
		public void AccountDetailsViewModel_ShouldReturnNull_WithInvalidId()
		{
			//Arrange
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			//Act & Assert
			Assert.That(async () => await accountService.GetAccountDetailsViewModel(Guid.NewGuid().ToString(), startDate, endDate),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountAndTransaction_WithValidInput()
		{
			//Arrange
			var inputModel = new AccountFormModel
			{
				Name = "AccountWithNonZeroBalance",
				AccountTypeId = this.AccountType1.Id,
				Balance = 100,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};

			int accountsCountBefore = data.Accounts.Count();

			//Act
			string newAccountId = await accountService.CreateAccount(inputModel);
			Account newAccount = await data.Accounts
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstAsync();

			//Assert
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore + 1));
			Assert.That(newAccount.Name, Is.EqualTo(inputModel.Name));
			Assert.That(newAccount.Balance, Is.EqualTo(inputModel.Balance));
			Assert.That(newAccount.CurrencyId, Is.EqualTo(inputModel.CurrencyId));
			Assert.That(newAccount.AccountTypeId, Is.EqualTo(inputModel.AccountTypeId));
			Assert.That(newAccount.Transactions.Count, Is.EqualTo(1));
			Assert.That(newAccount.Transactions.First().CategoryId, Is.EqualTo(this.Category1.Id));
		}

		[Test]
		public async Task CreateAccount_ShouldAddNewAccountWithoutTransaction_WithValidInput()
		{
			//Arrange
			AccountFormModel newAccountModel = new AccountFormModel
			{
				Name = "AccountWithZeroBalance",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};

			int accountsCountBefore = data.Accounts.Count();

			//Act
			string newAccountId = await accountService.CreateAccount(newAccountModel);
			Account? newAccount = data.Accounts
				.Where(a => a.Id == newAccountId)
				.Include(a => a.Transactions)
				.FirstOrDefault();

			//Assert
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore + 1));
			Assert.That(newAccount, Is.Not.Null);
			Assert.That(newAccount.Name, Is.EqualTo(newAccountModel.Name));
			Assert.That(newAccount.Balance, Is.EqualTo(newAccountModel.Balance));
			Assert.That(newAccount.CurrencyId, Is.EqualTo(newAccountModel.CurrencyId));
			Assert.That(newAccount.AccountTypeId, Is.EqualTo(newAccountModel.AccountTypeId));
			Assert.That(newAccount.Transactions.Any(), Is.False);
		}

		[Test]
		public async Task IsAccountOwner_ShouldReturnTrue_WhenUserIsOwner()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountOwner(this.User1.Id, this.Account1User1.Id);

			//Assert
			Assert.That(response, Is.True);
		}

		[Test]
		public async Task IsAccountOwner_ShouldReturnFalse_WhenUserIsNotOwner()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountOwner(this.User2.Id, this.Account1User1.Id);

			//Assert
			Assert.That(response, Is.False);
		}

		[Test]
		public void IsAccountOwner_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Assert
			Assert.That(async () => await accountService.IsAccountOwner(this.User1.Id, Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task IsAccountDeleted_ShouldReturnTrue_WhenAccountIsDeleted()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountDeleted(this.Account2User1.Id);

			//Assert
			Assert.That(response, Is.True);
		}

		[Test]
		public async Task IsAccountDeleted_ShouldReturnFalse_WhenAccountIsNotDeleted()
		{
			//Arrange & Act
			bool response = await accountService.IsAccountDeleted(this.Account1User1.Id);

			//Assert
			Assert.That(response, Is.False);
		}

		[Test]
		public void IsAccountDeleted_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.IsAccountDeleted(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccountById_ShouldDeleteAccountWithoutTransactions_WithValidId()
		{
			//Arrange
			string accountId = Guid.NewGuid().ToString();
			Account account = new Account
			{
				Id = accountId,
				Name = "AccountForDelete",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = accountId,
				Amount = 200,
				CategoryId = this.Category4.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			await data.Accounts.AddAsync(account);
			await data.Transactions.AddAsync(transaction);
			data.SaveChanges();

			//Assert that the Account and Transactions are created and Account is not deleted
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Act
			await accountService.DeleteAccount(accountId, false, this.User1.Id);

			//Assert that the Account is deleted but Transactions not
			Assert.That(account.IsDeleted, Is.True);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task DeleteAccountById_ShouldDeleteAccountAndTransactions_WithValidId()
		{
			//Arrange
			string accountId = Guid.NewGuid().ToString();
			Account account = new Account
			{
				Id = accountId,
				Name = "AccountForDelete",
				AccountTypeId = this.AccountType1.Id,
				Balance = 0,
				CurrencyId = this.Currency1.Id,
				OwnerId = this.User1.Id
			};
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = accountId,
				Amount = 200,
				CategoryId = this.Category4.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Salary",
				TransactionType = TransactionType.Income
			};
			await data.Accounts.AddAsync(account);
			await data.Transactions.AddAsync(transaction);
			data.SaveChanges();

			//Assert that the Account and Transactions are created and Account is not deleted
			Assert.That(account.IsDeleted, Is.False);
			Assert.That(account.Transactions.Count, Is.EqualTo(1));

			//Arrange
			int accountsCountBefore = data.Accounts.Count();
			int transactionsCountBefore = data.Transactions.Count();

			//Act
			await accountService.DeleteAccount(accountId, true, this.User1.Id);

			//Assert that the Account is deleted but Transactions not
			Assert.That(data.Accounts.Count(), Is.EqualTo(accountsCountBefore - 1));
			Assert.That(data.Transactions.Count(), Is.EqualTo(transactionsCountBefore - 1));
		}

		[Test]
		public void DeleteAccountById_ShouldThrowException_WhenIdIsInvalid()
		{
			//Arrange & Act

			//Assert
			Assert.That(async () => await accountService.DeleteAccount(Guid.NewGuid().ToString(), true, this.User1.Id),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccountViewModel_ShouldReturnCorrectViewModel_WithValidId()
		{
			//Act
			var actualViewModel = await accountService.GetDeleteAccountViewModel(this.Account1User1.Id);

			//Assert
			Assert.That(actualViewModel, Is.Not.Null);
			//Assert.That(actualViewModel.Id, Is.EqualTo(this.Account1User1.Id));
			Assert.That(actualViewModel.Name, Is.EqualTo(this.Account1User1.Name));
			//Assert.That(actualViewModel.OwnerId, Is.EqualTo(this.Account1User1.OwnerId));
		}

		[Test]
		public async Task CreateAccountType_ShouldAddNewAccountType_WithValidParams()
		{
			//Arrange
			var inputModel = new AccountTypeInputModel
			{
				Name = "NewAccountType",
				OwnerId = this.User1.Id
			};
			int countBefore = data.AccountTypes.Count();

			//Act
			AccountTypeViewModel viewModel = 
				await accountService.CreateAccountType(inputModel);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(viewModel.Id, Is.Not.Null);
			Assert.That(viewModel.Name, Is.EqualTo(inputModel.Name));
		}

		[Test]
		public async Task CreateAccountType_ShouldRecreateDeletedBeforeAccountType_WithValidParams()
		{
			//Arrange
			var deletedAccType = new AccountType
			{
				Id = Guid.NewGuid().ToString(),
				Name = "DeletedAccType",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await data.AccountTypes.AddAsync(deletedAccType);
			await data.SaveChangesAsync();
			int countBefore = data.AccountTypes.Count();
			
			var inputModel = new AccountTypeInputModel
			{
				Name = deletedAccType.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(async () =>
			{
				var deletedAcc = await data.AccountTypes.FindAsync(deletedAccType.Id);
				Assert.That(deletedAcc, Is.Not.Null);
				return deletedAcc.IsDeleted;
			}, Is.True);

			//Act
			AccountTypeViewModel viewModel = 
				await accountService.CreateAccountType(inputModel);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore));
			Assert.That(viewModel.Id, Is.Not.Null);
			Assert.That(viewModel.Name, Is.EqualTo(deletedAccType.Name));
		}

		[Test]
		public async Task CreateAccountType_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
		{
			//Arrange
			var user2AccType = new AccountType 
			{ 
				Id = Guid.NewGuid().ToString(),
				Name = "User2AccType", 
				OwnerId = this.User2.Id 
			};

			await data.AccountTypes.AddAsync(user2AccType);
			await data.SaveChangesAsync();
			int countBefore = data.AccountTypes.Count();
			
			var inputModel = new AccountTypeInputModel
			{
				Name = user2AccType.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(await data.AccountTypes.FindAsync(user2AccType.Id), Is.Not.Null);

			//Act
			AccountTypeViewModel viewModel = 
				await accountService.CreateAccountType(inputModel);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(viewModel.Id, Is.Not.Null);
			Assert.That(viewModel.Name, Is.EqualTo("User2AccType"));
		}

		[Test]
		public void CreateAccountType_ShouldThrowException_WhenAccTypeExist()
		{
			//Arrange
			
			var inputModel = new AccountTypeInputModel
			{
				Name = this.AccountType1.Name,
				OwnerId = this.User1.Id
			};

			//Act & Assert
			Assert.That(async () => await accountService.CreateAccountType(inputModel),
				Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Account Type with the same name exist!"));
		}

		//[Test]
		//[TestCase("A")]
		//[TestCase("NameWith16Chars!")]
		//public void CreateAccountType_ShouldThrowException_WithInvalidName(string accountTypeName)
		//{
		//	//Arrange
		//	var inputModel = new AccountTypeInputModel
		//	{
		//		Name = accountTypeName,
		//		OwnerId = this.User1.Id
		//	};

		//	//Act & Assert
		//	Assert.That(async () => await accountService.CreateAccountType(inputModel),
		//		Throws.TypeOf<ArgumentException>().With.Message
		//			.EqualTo("Account Type name must be between 2 and 15 characters long."));
		//}

		[Test]
		public async Task DeleteAccountType_ShouldRemoveAccType_WithValidParams()
		{
			//Arrange
			var newAccType = new AccountType() 
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewAccType", 
				OwnerId = this.User1.Id 
			};
			await data.AccountTypes.AddAsync(newAccType);
			await data.SaveChangesAsync();

			//Assert
			Assert.That(await data.AccountTypes.FindAsync(newAccType.Id), Is.Not.Null);
			Assert.That(newAccType.IsDeleted, Is.False);

			//Act
			await accountService.DeleteAccountType(newAccType.Id, this.User1.Id);

			//Assert
			Assert.That(newAccType.IsDeleted, Is.True);
		}

		[Test]
		public void DeleteAccountType_ShouldThrowException_WhenAccTypeNotExist()
		{
			//Act & Assert
			Assert.That(async () => await accountService.DeleteAccountType(Guid.NewGuid().ToString(), this.User1.Id),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccountType_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			var user2AccType = new AccountType() 
			{ 
				Id = Guid.NewGuid().ToString(),
				Name = "ForDelete", 
				OwnerId = this.User2.Id 
			};
			await data.AccountTypes.AddAsync(user2AccType);
			await data.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await accountService
					.DeleteAccountType(user2AccType.Id, this.User1.Id),
				Throws.TypeOf<ArgumentException>()
					.With.Message.EqualTo("Can't delete someone else Account Type."));
		}

		//[Test]
		//		public async Task CreateCurrency_ShouldAddNewCurrency_WithValidParams()
		//		{
		//			//Arrange
		//			int currenciesBefore = data.Currencies
		//				.Count(c => c.OwnerId == this.User1.Id && !c.IsDeleted);

		//			var model = new CurrencyViewModel { Name = "NEW" };

		//			//Assert
		//			Assert.That(await data.Currencies.FirstOrDefaultAsync(c => c.Name == model.Name),
		//				Is.Null);

		//			//Act
		//			await currencyService.CreateCurrency(this.User1.Id, model);
		//			int actualCurrencies = data.Currencies
		//				.Count(c => c.OwnerId == this.User1.Id && !c.IsDeleted);

		//			//Assert
		//			Assert.That(model.Id, Is.Not.Null);
		//			Assert.That(model.Name, Is.EqualTo("NEW"));
		//			Assert.That(model.OwnerId, Is.EqualTo(this.User1.Id));
		//			Assert.That(await data.Currencies.FirstOrDefaultAsync(c => c.Name == "NEW"), Is.Not.Null);
		//			Assert.That(actualCurrencies, Is.EqualTo(currenciesBefore + 1));
		//		}

		//		[Test]
		//		public async Task CreateCurrency_ShouldAddNewCurrencyWhenAnotherUserHaveTheSameCurrency()
		//		{
		//			//Arrange
		//			var user2Currency = new Currency { Name = "NEW2", OwnerId = this.User2.Id };
		//			data.Currencies.Add(user2Currency);
		//			await data.SaveChangesAsync();

		//			int currenciesBefore = data.Currencies.Count();
		//			var model = new CurrencyViewModel { Name = user2Currency.Name };

		//			//Assert
		//			Assert.That(await data.Currencies.FindAsync(user2Currency.Id), Is.Not.Null);

		//			//Act
		//			await currencyService.CreateCurrency(this.User1.Id, model);
		//			int actualCurrencies = data.Currencies.Count();

		//			//Assert
		//			Assert.That(model.Id, Is.Not.Null);
		//			Assert.That(model.Name, Is.EqualTo(user2Currency.Name));
		//			Assert.That(model.OwnerId, Is.EqualTo(this.User1.Id));
		//			Assert.That(data.Currencies.Count(c => c.Name == "NEW2"), Is.EqualTo(2));
		//			Assert.That(actualCurrencies, Is.EqualTo(currenciesBefore + 1));
		//		}

		//		[Test]
		//		public async Task CreateCurrency_ShouldRecreateDeletedBeforeCurrency_WithValidParams()
		//		{
		//			//Arrange
		//			var deletedCurrency = new Currency
		//			{
		//				Name = "DeletedCurrency",
		//				OwnerId = this.User1.Id,
		//				IsDeleted = true
		//			};
		//			data.Currencies.Add(deletedCurrency);
		//			await data.SaveChangesAsync();
		//			int countBefore = data.Currencies.Count();
		//			var model = new CurrencyViewModel { Name = deletedCurrency.Name };

		//			//Assert
		//			Assert.That(async () =>
		//			{
		//				var deletedCurr = await data.Currencies.FindAsync(deletedCurrency.Id);
		//				Assert.That(deletedCurr, Is.Not.Null);
		//				return deletedCurr.IsDeleted;
		//			}, Is.True);

		//			//Act
		//			await currencyService.CreateCurrency(this.User1.Id, model);
		//			int countAfter = data.Currencies.Count();

		//			//Assert
		//			Assert.That(model.Id, Is.Not.Null);
		//			Assert.That(countAfter, Is.EqualTo(countBefore));
		//			Assert.That(model.OwnerId, Is.EqualTo(this.User1.Id));
		//			Assert.That(model.Name, Is.EqualTo(deletedCurrency.Name));
		//		}

		//		[Test]
		//		public void CreateCurrency_ShouldThrowException_WithExistingCurrency()
		//		{
		//			//Arrange
		//			var model = new CurrencyViewModel { Name = this.Currency1.Name };

		//			//Act & Assert
		//			Assert.That(async () => await currencyService.CreateCurrency(this.User1.Id, model),
		//				Throws.TypeOf<ArgumentException>()
		//					.With.Message.EqualTo("Currency with the same name exist!"));
		//		}

		//		[Test]
		//		[TestCase("A")]
		//		[TestCase("NameWith11!")]
		//		public void CreateCurrency_ShouldThrowException_WithInvalidName(string currencyName)
		//		{
		//			//Arrange
		//			var model = new CurrencyViewModel { Name = currencyName };

		//			//Act & Assert
		//			Assert.That(async () => await currencyService.CreateCurrency(this.User1.Id, model),
		//				Throws.TypeOf<ArgumentException>().With.Message
		//					.EqualTo("Currency name must be between 2 and 10 characters long."));
		//		}

		//		[Test]
		//		public async Task DeleteCurrency_ShouldDeleteCurrency_WithValidParams()
		//		{
		//			//Arrange
		//			var newCurrency = new Currency { Name = "DEL", OwnerId = this.User1.Id };
		//			data.Currencies.Add(newCurrency);
		//			await data.SaveChangesAsync();

		//			//Assert
		//			Assert.That(await data.Currencies.FindAsync(newCurrency.Id), Is.Not.Null);
		//			Assert.That(newCurrency.IsDeleted, Is.False);

		//			//Act
		//			await currencyService.DeleteCurrency(newCurrency.Id, this.User1.Id);

		//			//Assert
		//			Assert.That(newCurrency.IsDeleted, Is.True);
		//		}

		//		[Test]
		//		public void DeleteCurrency_ShouldThrowException_WhenCurrencyNotExist()
		//		{
		//			//Arrange

		//			//Act & Assert
		//			Assert.That(async () => await currencyService.DeleteCurrency(Guid.NewGuid().ToString(), this.User1.Id),
		//				Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Currency does not exist."));
		//		}

		//		[Test]
		//		public async Task DeleteCurrency_ShouldThrowException_WhenUserIsNotOwner()
		//		{
		//			//Arrange
		//			var newCurrency = new Currency { Name = "NOT", OwnerId = this.User2.Id };
		//			data.Currencies.Add(newCurrency);
		//			await data.SaveChangesAsync();

		//			//Act & Assert
		//			Assert.That(async () => await currencyService.DeleteCurrency(newCurrency.Id, this.User1.Id),
		//				Throws.TypeOf<InvalidOperationException>()
		//					.With.Message.EqualTo("Can't delete someone else Currency."));
		//		}
	}
}
