namespace PersonalFinancer.Tests.Services
{
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using System.Linq.Expressions;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	[TestFixture]
	internal class AccountsInfoServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<Transaction> transactionsRepo;
		private IEfRepository<Account> accountsRepo;
		private IAccountsInfoService accountsInfoService;

		[SetUp]
		public void SetUp()
		{
			this.transactionsRepo = new EfRepository<Transaction>(this.dbContextMock);
			this.accountsRepo = new EfRepository<Account>(this.dbContextMock);

			this.accountsInfoService = new AccountsInfoService(
				this.transactionsRepo, this.accountsRepo, this.mapperMock);
		}

		[Test]
		public async Task GetAccountTransactions_ShouldReturnCorrectData()
		{
			//Arrange
			var dto = new AccountTransactionsFilterDTO
			{
				AccountId = this.Account1_User1_WithTransactions.Id,
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now,
				Page = 1
			};

			Expression<Func<Transaction, bool>> filter = (t) =>
				t.AccountId == this.Account1_User1_WithTransactions.Id
				&& t.CreatedOn >= dto.StartDate.ToUniversalTime() 
				&& t.CreatedOn <= dto.EndDate.ToUniversalTime();

			var expected = new TransactionsDTO
			{
				Transactions = await this.transactionsRepo.All()
					.Where(filter)
					.OrderByDescending(t => t.CreatedOn)
					.Take(TransactionsPerPage)
					.ProjectTo<TransactionTableDTO>(this.mapperMock.ConfigurationProvider)
					.ToListAsync(),
				TotalTransactionsCount = await this.transactionsRepo.All()
					.CountAsync(filter)
			};

			//Act
			TransactionsDTO actual = await this.accountsInfoService.GetAccountTransactionsAsync(dto);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public void GetAccountTransactions_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			var dto = new AccountTransactionsFilterDTO
			{
				AccountId = Guid.NewGuid(),
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow,
				Page = 1
			};

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountTransactionsAsync(dto), 
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountDetails_ShouldReturnAccountDetails_WhenUserIsOwner()
		{
			//Arrange
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			AccountDetailsLongDTO expected = await this.accountsRepo.All()
				.Where(a => a.Id == this.Account1_User1_WithTransactions.Id && !a.IsDeleted)
				.ProjectTo<AccountDetailsLongDTO>(this.mapperMock.ConfigurationProvider, new { startDate, endDate })
				.FirstAsync();

			//Act
			AccountDetailsLongDTO actual = await this.accountsInfoService.GetAccountDetailsAsync(
				this.Account1_User1_WithTransactions.Id, startDate, endDate, this.User1.Id, isUserAdmin: false);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public void GetAccountDetails_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			var id = Guid.NewGuid();
			DateTime startDate = DateTime.UtcNow.AddMonths(-1);
			DateTime endDate = DateTime.UtcNow;

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountDetailsAsync(id, startDate, endDate, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountCardsData_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = new AccountsCardsDTO
			{
				Accounts = await this.accountsRepo.All()
					.Where(a => !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Take(AccountsPerPage)
					.ProjectTo<AccountCardDTO>(this.mapperMock.ConfigurationProvider)
					.ToArrayAsync(),
				TotalAccountsCount = await this.accountsRepo.All()
					.CountAsync(a => !a.IsDeleted)
			};

			//Act
			AccountsCardsDTO actual = await this.accountsInfoService.GetAccountsCardsDataAsync(1);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetCashFlowByCurrenciesAsync_ShouldReturnCorrectData()
		{
			//Arrange
			IEnumerable<CurrencyCashFlowDTO> expected = await this.accountsRepo.All()
				.Where(a => a.Transactions.Any())
				.GroupBy(a => a.Currency.Name)
				.Select(group => new CurrencyCashFlowDTO
				{
					Name = group.Key,
					Incomes = group
						.Sum(a => a.Transactions
							.Where(t => t.TransactionType == TransactionType.Income)
							.Sum(t => t.Amount)),
					Expenses = group
						.Sum(a => a.Transactions
							.Where(t => t.TransactionType == TransactionType.Expense)
							.Sum(t => t.Amount))
				})
				.ToArrayAsync();

			//Act
			IEnumerable<CurrencyCashFlowDTO> actual =
				await this.accountsInfoService.GetCashFlowByCurrenciesAsync();

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetAccountName_ShouldReturnAccountName_WithValidId()
		{
			//Act
			string actualName = await this.accountsInfoService
				.GetAccountNameAsync(this.Account1_User1_WithTransactions.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(actualName, Is.EqualTo(this.Account1_User1_WithTransactions.Name));
		}

		[Test]
		public void GetAccountName_ShouldThrowException_WithInvalidId()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountNameAsync(invalidId, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountsCountAsync_ShouldReturnAccountsCount()
		{
			//Arrange
			int expectedCount = await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

			//Act
			int actualCount = await this.accountsInfoService.GetAccountsCountAsync();

			//Assert
			Assert.That(actualCount, Is.EqualTo(expectedCount));
		}

		[Test]
		public async Task GetAccountFormData_ShouldReturnCorrectData()
		{
			//Arrange
			var expected = this.mapperMock.Map<CreateEditAccountDTO>(this.Account1_User1_WithTransactions);

			//Act
			CreateEditAccountDTO actual = await this.accountsInfoService
				.GetAccountFormDataAsync(this.Account1_User1_WithTransactions.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetAccountFormData_ShouldReturnCorrectData_WhenUserIsAdmin()
		{
			//Arrange
			var expected = this.mapperMock.Map<CreateEditAccountDTO>(this.Account1_User1_WithTransactions);

			//Act
			CreateEditAccountDTO actual = await this.accountsInfoService
				.GetAccountFormDataAsync(this.Account1_User1_WithTransactions.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public void GetAccountFormData_ShouldThrowException_WhenAccountDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountFormDataAsync(invalidId, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void GetAccountFormData_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountFormDataAsync(this.Account1_User1_WithTransactions.Id, this.User2.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionFormData_ShouldReturnCorrectData_WhenTransactionIsNotInitial()
		{
			//Arrange
			CreateEditTransactionDTO expected = await this.transactionsRepo.All()
				.Where(t => t.Id == this.Transaction1_Expense_Account1_User1.Id)
				.ProjectTo<CreateEditTransactionDTO>(this.mapperMock.ConfigurationProvider)
				.FirstAsync();

			//Act
			CreateEditTransactionDTO actual = await this.accountsInfoService
				.GetTransactionFormDataAsync(this.Transaction1_Expense_Account1_User1.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public void GetTransactionFormData_ShouldThrowException_WhenTransactionIsInitial()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionFormDataAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User1.Id, isUserAdmin: true),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void GetFulfilledTransactionFormModel_ShouldThrowException_WhenTransactionDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionFormDataAsync(invalidId, this.User1.Id, isUserAdmin: true),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetOwnerId_ShouldReturnOwnerId_WithValidAccountId()
		{
			//Act
			Guid ownerId = await this.accountsInfoService.GetAccountOwnerIdAsync(this.Account1_User1_WithTransactions.Id);

			//Assert
			Assert.That(ownerId, Is.EqualTo(this.User1.Id));
		}

		[Test]
		public void GetOwnerId_ShouldThrowException_WhenAccountIdIsInvalid()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountOwnerIdAsync(invalidId),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Arrange
			TransactionDetailsDTO expected = await this.transactionsRepo.All()
				.Where(t => t.Id == this.InitialTransaction_Income_Account1_User1.Id)
				.ProjectTo<TransactionDetailsDTO>(this.mapperMock.ConfigurationProvider)
				.FirstAsync();

			//Act
			TransactionDetailsDTO actual = await this.accountsInfoService
				.GetTransactionDetailsAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public async Task GetTransactionDetails_ShouldReturnCorrectDTO_WhenUserIsAdmin()
		{
			//Arrange
			TransactionDetailsDTO expected = await this.transactionsRepo.All()
				.Where(t => t.Id == this.InitialTransaction_Income_Account1_User1.Id)
				.ProjectTo<TransactionDetailsDTO>(this.mapperMock.ConfigurationProvider)
				.FirstAsync();

			//Act
			TransactionDetailsDTO actual = await this.accountsInfoService
				.GetTransactionDetailsAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public void GetTransactionDetails_ShouldThrowException_WithInValidTransactionId()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionDetailsAsync(invalidId, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public void GetTransactionDetails_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionDetailsAsync(this.InitialTransaction_Income_Account1_User1.Id, this.User2.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task GetAccountShortDetails_ShouldReturnCorrectData_WithValidInput()
		{
			//Arrange
			AccountDetailsShortDTO expected = await this.accountsRepo.All()
				.Where(a => a.Id == this.Account1_User1_WithTransactions.Id)
				.ProjectTo<AccountDetailsShortDTO>(this.mapperMock.ConfigurationProvider)
				.FirstAsync();

			//Act
			AccountDetailsShortDTO actual = await this.accountsInfoService
				.GetAccountShortDetailsAsync(this.Account1_User1_WithTransactions.Id);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}

		[Test]
		public void GetAccountShortDetails_ShouldThrowException_WithInvalidInput()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountShortDetailsAsync(invalidId),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetUserAccounts_ShouldReturnUsersAccounts_WithValidId()
		{
			//Arrange
			List<AccountCardDTO> expected = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.ProjectTo<AccountCardDTO>(this.mapperMock.ConfigurationProvider)
				.ToListAsync();

			//Act
			IEnumerable<AccountCardDTO> actual = await this.accountsInfoService.GetUserAccountsCardsAsync(this.User1.Id);

			//Assert
			this.AssertAreEqualAsJson(actual, expected);
		}
	}
}
