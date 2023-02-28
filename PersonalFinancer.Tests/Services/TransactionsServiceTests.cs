namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;

	using PersonalFinancer.Data.Enums;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Transactions;
	using PersonalFinancer.Services.Transactions.Models;

	internal class TransactionsServiceTests : UnitTestsBase
	{
		private ITransactionsService transactionService;

		[SetUp]
		public void SetUp()
		{
			this.transactionService = new TransactionsService(this.data, this.mapper);
		}

		[Test]
		public async Task CreateTransaction_ShouldAddNewTransaction_AndChangeAccountBalance()
		{
			//Arrange
			TransactionFormModel transactionModel = new TransactionFormModel()
			{
				Amount = 100,
				AccountId = this.Account1User1.Id,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Not Initial Balance",
				TransactionType = TransactionType.Expense
			};
			int transactionsCountBefore = this.Account1User1.Transactions.Count();
			decimal balanceBefore = this.Account1User1.Balance;

			//Act
			Guid id = await transactionService.CreateTransaction(transactionModel);
			Transaction? transaction = await data.Transactions.FindAsync(id);

			//Assert
			Assert.That(transaction, Is.Not.Null);
			Assert.That(this.Account1User1.Transactions.Count(), Is.EqualTo(transactionsCountBefore + 1));
			Assert.That(transaction.Amount, Is.EqualTo(transactionModel.Amount));
			Assert.That(transaction.CategoryId, Is.EqualTo(transactionModel.CategoryId));
			Assert.That(transaction.AccountId, Is.EqualTo(transactionModel.AccountId));
			Assert.That(transaction.Refference, Is.EqualTo(transactionModel.Refference));
			Assert.That(transaction.CreatedOn, Is.EqualTo(transactionModel.CreatedOn));
			Assert.That(transaction.TransactionType, Is.EqualTo(transactionModel.TransactionType));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount));
		}

		[Test]
		public async Task CreateTransaction_ShouldAddNewTransaction_WithoutChangeAccountBalance()
		{
			//Arrange
			TransactionFormModel transactionModel = new TransactionFormModel()
			{
				Amount = 100,
				AccountId = this.Account1User1.Id,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "Initial Balance",
				TransactionType = TransactionType.Expense
			};
			int transactionsCountBefore = this.Account1User1.Transactions.Count();
			decimal balanceBefore = this.Account1User1.Balance;

			//Act
			Guid id = await transactionService.CreateTransaction(transactionModel, true);
			Transaction? transaction = await data.Transactions.FindAsync(id);

			//Assert
			Assert.That(transaction, Is.Not.Null);
			Assert.That(this.Account1User1.Transactions.Count(), Is.EqualTo(transactionsCountBefore + 1));
			Assert.That(transaction.Amount, Is.EqualTo(transactionModel.Amount));
			Assert.That(transaction.CategoryId, Is.EqualTo(transactionModel.CategoryId));
			Assert.That(transaction.AccountId, Is.EqualTo(transactionModel.AccountId));
			Assert.That(transaction.Refference, Is.EqualTo(transactionModel.Refference));
			Assert.That(transaction.CreatedOn, Is.EqualTo(transactionModel.CreatedOn));
			Assert.That(transaction.TransactionType, Is.EqualTo(transactionModel.TransactionType));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore));
		}

		[Test]
		public async Task DeleteTransactionById_ShouldDeleteExpenseTransactionIncreaseBalanceAndReturnTrue_WithValidInput()
		{
			//Arrange
			Guid transactionId = Guid.NewGuid();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Expense
			};
			data.Transactions.Add(transaction);
			this.Account1User1.Balance -= transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = this.Account1User1.Balance;
			int transactionsBefore = this.Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			bool response = await transactionService.DeleteTransactionById(transactionId);

			//Assert
			Assert.That(response, Is.True);
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
			Assert.That(this.Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public async Task DeleteTransactionById_ShouldDeleteIncomeTransactionReductBalanceAndReturnTrue_WithValidInput()
		{
			//Arrange
			Guid transactionId = Guid.NewGuid();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Income
			};
			data.Transactions.Add(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = this.Account1User1.Balance;
			int transactionsBefore = this.Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			bool response = await transactionService.DeleteTransactionById(transactionId);

			//Assert
			Assert.That(response, Is.True);
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
			Assert.That(this.Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public async Task DeleteTransactionById_ShouldReturnFalse_WithInvalidInput()
		{
			//Arrange

			//Act & Assert
			Assert.That(await transactionService.DeleteTransactionById(Guid.NewGuid()), Is.False);
		}

		[Test]
		public async Task EditTransactionFormModelById_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			EditTransactionFormModel? transactionFormModel = await transactionService
				.EditTransactionFormModelById(this.Transaction1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1.Id));
			Assert.That(transactionFormModel.AccountId, Is.EqualTo(this.Transaction1.AccountId));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1.Amount));
			Assert.That(transactionFormModel.CategoryId, Is.EqualTo(this.Transaction1.CategoryId));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(this.Transaction1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1.TransactionType));
		}

		[Test]
		public async Task EditTransactionFormModelById_ShouldReturnNull_WithInValidInput()
		{
			//Act & Assert
			Assert.That(await transactionService.EditTransactionFormModelById(Guid.NewGuid()), Is.Null);
		}

		[Test]
		public async Task TransactionViewModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionExtendedViewModel? transactionFormModel = await transactionService
				.TransactionViewModel(this.Transaction1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1.Id));
			Assert.That(transactionFormModel.AccountName, Is.EqualTo(this.Transaction1.Account.Name));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1.Amount));
			Assert.That(transactionFormModel.CategoryName, Is.EqualTo(this.Transaction1.Category.Name));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(this.Transaction1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1.TransactionType.ToString()));
		}

		[Test]
		public async Task TransactionViewModel_ShouldReturnNull_WithInValidInput()
		{
			//Act & Assert
			Assert.That(await transactionService.TransactionViewModel(Guid.NewGuid()), Is.Null);
		}

		[Test]
		public async Task AllTransactionsViewModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Arrange
			AllTransactionsServiceModel model = new AllTransactionsServiceModel
			{
				StartDate = DateTime.Now.AddMonths(-1),
				EndDate = DateTime.Now
			};

			IEnumerable<Transaction> expectedTransactions = await data.Transactions
				.Where(t => t.Account.OwnerId == this.User1.Id &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.ToListAsync();

			//Act
			AllTransactionsServiceModel? transactionFormModel = await transactionService
				.AllTransactionsViewModel(this.User1.Id, model);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			Assert.That(transactionFormModel.Transactions.Count(), Is.EqualTo(expectedTransactions.Count()));
			for (int i = 0; i < expectedTransactions.Count(); i++)
			{
				Assert.That(transactionFormModel.Transactions.ElementAt(i).Id, 
					Is.EqualTo(expectedTransactions.ElementAt(i).Id));
				Assert.That(transactionFormModel.Transactions.ElementAt(i).AccountName, 
					Is.EqualTo(expectedTransactions.ElementAt(i).Account.Name));
				Assert.That(transactionFormModel.Transactions.ElementAt(i).Amount, 
					Is.EqualTo(expectedTransactions.ElementAt(i).Amount));
				Assert.That(transactionFormModel.Transactions.ElementAt(i).CategoryName, 
					Is.EqualTo(expectedTransactions.ElementAt(i).Category.Name));
				Assert.That(transactionFormModel.Transactions.ElementAt(i).Refference, 
					Is.EqualTo(expectedTransactions.ElementAt(i).Refference));
				Assert.That(transactionFormModel.Transactions.ElementAt(i).TransactionType, 
					Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
			}
		}

		[Test]
		public void AllTransactionsViewModel_ShouldThrowException_WithInValidInput()
		{
			//Arrange
			AllTransactionsServiceModel model = new AllTransactionsServiceModel
			{
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddMonths(-1),
			};

			//Assert
			Assert.That(async () => await transactionService.AllTransactionsViewModel(this.User1.Id, model),
				Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Start Date must be before End Date."));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalance_WhenTransactionTypeIsChanged()
		{
			//Arrange
			Guid transactionId = Guid.NewGuid();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TransactionTypeChanged",
				TransactionType = TransactionType.Income
			};

			data.Transactions.Add(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal balanceBefore = this.Account1User1.Balance;

			EditTransactionFormModel transactionEditModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<EditTransactionFormModel>(t))
				.FirstAsync();
			
			//Act
			transactionEditModel.TransactionType = TransactionType.Expense;
			await transactionService.EditTransaction(transactionEditModel);

			//Assert
			Assert.That(transaction.TransactionType, Is.EqualTo(TransactionType.Expense));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount * 2));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalanceOnTwoAccounts_WhenAccountIsChanged()
		{
			//Arrange
			Guid transactionId = Guid.NewGuid();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = this.Account2User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "AccountChanged",
				TransactionType = TransactionType.Income
			};

			data.Transactions.Add(transaction);
			this.Account2User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal firstAccBalanceBefore = this.Account2User1.Balance;
			decimal secondAccBalanceBefore = this.Account1User1.Balance;

			//Act
			EditTransactionFormModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<EditTransactionFormModel>(t))
				.FirstAsync();
			editTransactionModel.AccountId = this.Account1User1.Id;
			await transactionService.EditTransaction(editTransactionModel);

			//Assert
			Assert.That(transaction.AccountId, Is.EqualTo(this.Account1User1.Id));
			Assert.That(this.Account2User1.Balance, Is.EqualTo(firstAccBalanceBefore - transaction.Amount));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(secondAccBalanceBefore + transaction.Amount));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransaction_WhenPaymentRefferenceIsChanged()
		{
			//Arrange
			Guid transactionId = Guid.NewGuid();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "First Refference",
				TransactionType = TransactionType.Income
			};

			data.Transactions.Add(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal balanceBefore = this.Account1User1.Balance;
			Guid categoryIdBefore = transaction.CategoryId;

			//Act
			EditTransactionFormModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<EditTransactionFormModel>(t))
				.FirstAsync();
			editTransactionModel.Refference = "Second Refference";
			await transactionService.EditTransaction(editTransactionModel);

			//Assert that only transaction refference is changed
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore));
			Assert.That(transaction.Refference, Is.EqualTo(editTransactionModel.Refference));
			Assert.That(transaction.CategoryId, Is.EqualTo(categoryIdBefore));
		}
	}
}
