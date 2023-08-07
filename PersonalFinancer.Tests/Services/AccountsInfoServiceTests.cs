namespace PersonalFinancer.Tests.Services
{
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using Newtonsoft.Json;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
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
			this.transactionsRepo = new EfRepository<Transaction>(this.dbContext);
			this.accountsRepo = new EfRepository<Account>(this.dbContext);

			this.accountsInfoService = new AccountsInfoService(
				this.transactionsRepo, this.accountsRepo, this.mapper);
		}

		[Test]
		[TestCase(1, null)]
		[TestCase(1, "user0")]
		[TestCase(1, "account0")]
		[TestCase(1, "Currency0")]
		[TestCase(1, "currency0")]
		public async Task GetAccountsCardsDataAsync_ShouldReturnCorrectData(int page, string? search)
		{
			//Arrange
			var query = this.accountsRepo.All().Where(a => !a.IsDeleted);

			if (search != null)
			{
				string expectedSearch = search.ToLower();

				query = query.Where(a =>
					a.Name.ToLower().Contains(expectedSearch) ||
					a.Currency.Name.ToLower().Contains(expectedSearch) ||
					a.AccountType.Name.ToLower().Contains(expectedSearch));
			}

			var expected = new AccountsCardsDTO
			{
				Accounts = await query
					.OrderBy(a => a.Name)
					.Skip(AccountsPerPage * (page - 1))
					.Take(AccountsPerPage)
					.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalAccountsCount = await query.CountAsync()
			};

			//Act
			AccountsCardsDTO actual = await this.accountsInfoService.GetAccountsCardsDataAsync(page, search);

			//Assert
			Assert.That(JsonConvert.SerializeObject(actual), 
				Is.EqualTo(JsonConvert.SerializeObject(expected)));
		}

		[Test]
		public async Task GetAccountDetailsAsync_ShouldReturnCorrectData_WhenUserIsOwner()
		{
			//Arrange
			AccountDetailsDTO expected = await this.accountsRepo.All()
				.Where(a => !a.IsDeleted && !a.Owner.IsAdmin)
				.ProjectTo<AccountDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			//Act
			AccountDetailsDTO actual = await this.accountsInfoService.GetAccountDetailsAsync(
				expected.Id, expected.OwnerId, isUserAdmin: false);

			//Assert
			Assert.That(JsonConvert.SerializeObject(actual), 
				Is.EqualTo(JsonConvert.SerializeObject(expected)));
		}

		[Test]
		public void GetAccountDetailsAsync_ShouldThrowInvalidOperationException_WhenAccountDoesNotExist()
		{
			//Arrange
			var id = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountDetailsAsync(id, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountDetailsAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Guid accountId = await this.accountsRepo.All()
				.Where(a => a.OwnerId != this.mainTestUserId && !a.IsDeleted)
				.Select(a => a.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountDetailsAsync(accountId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task GetAccountFormDataAsync_ShouldReturnCorrectData(bool isUserAdmin)
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All().FirstAsync();
			var expected = this.mapper.Map<CreateEditAccountOutputDTO>(testAccount);
			Guid currentUserId = isUserAdmin ? this.adminId : testAccount.OwnerId;

			//Act
			var actual = await this.accountsInfoService
				.GetAccountFormDataAsync(testAccount.Id, currentUserId, isUserAdmin);

			//Assert
			Assert.That(JsonConvert.SerializeObject(actual), 
				Is.EqualTo(JsonConvert.SerializeObject(expected)));
		}

		[Test]
		public void GetAccountFormDataAsync_ShouldThrowInvalidOperationException_WhenAccountDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountFormDataAsync(invalidId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountFormDataAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Guid testAccountId = await this.accountsRepo.All()
				.Where(a => a.OwnerId != this.mainTestUserId)
				.Select(a => a.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountFormDataAsync(testAccountId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task GetAccountNameAsync_ShouldReturnAccountName_WhenAccountIsValid()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.OwnerId == this.mainTestUserId)
				.FirstAsync();

			//Act
			string actualName = await this.accountsInfoService
				.GetAccountNameAsync(testAccount.Id, this.mainTestUserId, isUserAdmin: false);

			//Assert
			Assert.That(actualName, Is.EqualTo(testAccount.Name));
		}

		[Test]
		public void GetAccountNameAsync_ShouldThrowInvalidOperationException_WhenAccountIsInvalid()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountNameAsync(invalidId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetAccountNameAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Guid accountId = await this.accountsRepo.All()
				.Where(a => a.OwnerId != this.mainTestUserId && !a.IsDeleted)
				.Select(a => a.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetAccountNameAsync(accountId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
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
		public async Task GetAccountTransactionsAsync_ShouldReturnCorrectData()
		{
			//Arrange
			Account testAccount = await this.accountsRepo.All()
				.Where(a => a.Transactions.Any())
				.Include(a => a.Transactions)
				.FirstAsync();

			var filterDto = new AccountTransactionsFilterDTO
			{
				Id = testAccount.Id,
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now,
				Page = 1
			};

			Func<Transaction, bool> filter = (t) =>
				t.CreatedOnUtc >= filterDto.FromLocalTime.ToUniversalTime() 
				&& t.CreatedOnUtc <= filterDto.ToLocalTime.ToUniversalTime();

			var expected = new TransactionsDTO
			{
				Transactions = testAccount.Transactions
					.Where(filter)
					.OrderByDescending(t => t.CreatedOnUtc)
					.Take(TransactionsPerPage)
					.Select(t => this.mapper.Map<TransactionTableDTO>(t)),
				TotalTransactionsCount = testAccount.Transactions.Count(filter)
			};

			//Act
			TransactionsDTO actual = await this.accountsInfoService.GetAccountTransactionsAsync(filterDto);

			//Assert
			Assert.That(JsonConvert.SerializeObject(actual), 
				Is.EqualTo(JsonConvert.SerializeObject(expected)));
		}

		[Test]
		public void GetAccountTransactionsAsync_ShouldThrowInvalidOperationException_WhenAccountDoesNotExist()
		{
			//Arrange
			var dto = new AccountTransactionsFilterDTO
			{
				Id = Guid.NewGuid(),
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now,
				Page = 1
			};

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService.GetAccountTransactionsAsync(dto), 
			Throws.TypeOf<InvalidOperationException>());
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
			Assert.That(JsonConvert.SerializeObject(actual), 
				Is.EqualTo(JsonConvert.SerializeObject(expected)));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task GetTransactionDetailsAsync_ShouldReturnCorrectData_WhenTransactionIsValid(bool isUserAdmin)
		{
			//Arrange
			Transaction testTransaction = await this.transactionsRepo.All()
				.FirstAsync(t => t.OwnerId == this.mainTestUserId);

			TransactionDetailsDTO expected = this.mapper.Map<TransactionDetailsDTO>(testTransaction);

			Guid currentUserId = isUserAdmin ? this.adminId : testTransaction.OwnerId;

			//Act
			TransactionDetailsDTO actual = await this.accountsInfoService
				.GetTransactionDetailsAsync(testTransaction.Id, currentUserId, isUserAdmin);

			//Assert
			Assert.That(JsonConvert.SerializeObject(actual), 
				Is.EqualTo(JsonConvert.SerializeObject(expected)));
		}

		[Test]
		public void GetTransactionDetailsAsync_ShouldThrowInvalidOperationException_WhenTransactionDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionDetailsAsync(invalidId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionDetailsAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwner()
		{
			//Arrange
			Guid testTransactionId = await this.transactionsRepo.All()
				.Where(t => t.OwnerId != this.mainTestUserId)
				.Select(t => t.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionDetailsAsync(testTransactionId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}

		[Test]
		public async Task GetTransactionFormDataAsync_ShouldReturnCorrectData_WhenTransactionIsNotInitialAndUserIsOwner()
		{
			//Arrange
			Transaction testTransaction = await this.transactionsRepo.All()
				.FirstAsync(t => t.OwnerId == this.mainTestUserId && !t.IsInitialBalance);

			CreateEditTransactionOutputDTO expected = this.mapper.Map<CreateEditTransactionOutputDTO>(testTransaction);

			//Act
			CreateEditTransactionOutputDTO actual = await this.accountsInfoService
				.GetTransactionFormDataAsync(testTransaction.Id, testTransaction.OwnerId, isUserAdmin: false);

			//Assert
			Assert.That(JsonConvert.SerializeObject(actual), 
				Is.EqualTo(JsonConvert.SerializeObject(expected)));
		}

		[Test]
		public void GetTransactionFormDataAsync_ShouldThrowInvalidOperationException_WhenTheTransactionDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionFormDataAsync(invalidId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionFormDataAsync_ShouldThrowInvalidOperationException_WhenTheTransactionIsInitial()
		{
			//Arrange
			Guid testTransactionId = await this.transactionsRepo.All()
				.Where(t => t.OwnerId == this.mainTestUserId && t.IsInitialBalance)
				.Select(t => t.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionFormDataAsync(testTransactionId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTransactionFormDataAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			Guid testTransactionId = await this.transactionsRepo.All()
				.Where(t => t.OwnerId != this.mainTestUserId && !t.IsInitialBalance)
				.Select(t => t.Id)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountsInfoService
				  .GetTransactionFormDataAsync(testTransactionId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>());
		}
	}
}
