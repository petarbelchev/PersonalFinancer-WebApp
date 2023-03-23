using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	internal class TransactionsServiceTests : UnitTestsBase
	{
		private ITransactionsService transactionService;

		[SetUp]
		public void SetUp()
		{
			this.transactionService = new TransactionsService(this.data, this.mapper, memoryCache);
		}

		[Test]
		public async Task CreateCategory_ShouldAddNewCategory_WithValidData()
		{
			//Arrange
			var inputModel = new CategoryInputModel
			{
				Name = "NewCategory",
				OwnerId = this.User1.Id
			};

			//Act
			CategoryViewModel viewModel = await transactionService.CreateCategory(inputModel);

			//Assert
			Assert.That(viewModel.Id, Is.Not.Null);
			Assert.That(viewModel.Name, Is.EqualTo(inputModel.Name));
		}

		[Test]
		public void CreateCategory_ShouldThrowException_WithExistingName()
		{
			//Arrange
			var inputModel = new CategoryInputModel
			{
				Name = this.Category2.Name,
				OwnerId = this.User1.Id
			};

			//Act & Assert
			Assert.That(async () => await transactionService.CreateCategory(inputModel),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Category with the same name exist!"));
		}

		[Test]
		public async Task CreateCategory_ShouldRecreateDeletedCategory_WithValidData()
		{
			//Arrange: Add deleted Category to database
			Category category = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "DeletedCategory",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			data.Categories.Add(category);
			data.SaveChanges();

			var inputModel = new CategoryInputModel
			{
				Name = category.Name,
				OwnerId = this.User1.Id
			};

			//Assert: The Category is deleted
			Assert.That(category.IsDeleted, Is.True);

			//Act: Recreate deleted Category
			CategoryViewModel newCategory = 
				await transactionService.CreateCategory(inputModel);

			//Assert: The Category is not deleted anymore and the data is correct
			Assert.That(category.IsDeleted, Is.False);
			Assert.That(newCategory.Id, Is.EqualTo(category.Id));
			Assert.That(newCategory.Name, Is.EqualTo(category.Name));
		}

		[Test]
		public async Task DeleteCategory_ShouldDeleteCategory_WithValidData()
		{
			//Arrange
			Category category = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "TestCategory",
				OwnerId = this.User1.Id
			};
			data.Categories.Add(category);
			data.SaveChanges();

			//Assert that the Category is not deleted
			Assert.That(category.IsDeleted, Is.False);

			//Act
			await this.transactionService.DeleteCategory(category.Id, this.User1.Id);

			//Assert that the Category is deleted
			Assert.That(category.IsDeleted, Is.True);
		}

		[Test]
		public void DeleteCategory_ShouldThrowException_WithInvalidCategoryId()
		{
			//Act & Assert
			Assert.That(async () => await transactionService.DeleteCategory(Guid.NewGuid().ToString(), this.User1.Id),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteCategory_ShouldThrowException_WithInvalidUserId()
		{
			//Arrange
			Category category = new Category
			{
				Id = Guid.NewGuid().ToString(),
				Name = "TestCategory",
				OwnerId = this.User1.Id
			};
			await data.Categories.AddAsync(category);
			await data.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await transactionService.DeleteCategory(category.Id, this.User2.Id),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Can't delete someone else category."));
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
			string id = await transactionService.CreateTransaction(this.User1.Id, transactionModel);
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

		//[Test]
		//public async Task DeleteTransactionById_ShouldDeleteExpenseTransactionIncreaseBalanceAndNewBalance_WithValidInput()
		//{
		//	//Arrange
		//	string transactionId = Guid.NewGuid().ToString();
		//	Transaction transaction = new Transaction
		//	{
		//		Id = transactionId,
		//		AccountId = this.Account1User1.Id,
		//		Amount = 123,
		//		CategoryId = this.Category2.Id,
		//		CreatedOn = DateTime.UtcNow,
		//		Refference = "TestTransaction",
		//		TransactionType = TransactionType.Expense
		//	};
		//	data.Transactions.Add(transaction);
		//	this.Account1User1.Balance -= transaction.Amount;
		//	await data.SaveChangesAsync();

		//	decimal balanceBefore = this.Account1User1.Balance;
		//	int transactionsBefore = this.Account1User1.Transactions.Count;
		//	Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

		//	//Assert
		//	Assert.That(transactionInDb, Is.Not.Null);

		//	//Act
		//	decimal newBalance = await transactionService.DeleteTransaction(transactionId);

		//	//Assert
		//	Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore + transactionInDb.Amount));
		//	Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
		//	Assert.That(this.Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
		//	Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		//}

		[Test]
		public async Task DeleteTransactionById_ShouldDeleteIncomeTransactionReductBalanceAndNewBalance_WithValidInput()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "TestTransaction",
				TransactionType = TransactionType.Income
			};
			await data.Transactions.AddAsync(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();

			decimal balanceBefore = this.Account1User1.Balance;
			int transactionsBefore = this.Account1User1.Transactions.Count;
			Transaction? transactionInDb = await data.Transactions.FindAsync(transactionId);

			//Assert
			Assert.That(transactionInDb, Is.Not.Null);

			//Act
			decimal newBalance = await transactionService.DeleteTransaction(transactionId);

			//Assert
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transactionInDb.Amount));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(newBalance));
			Assert.That(this.Account1User1.Transactions.Count, Is.EqualTo(transactionsBefore - 1));
			Assert.That(await data.Transactions.FindAsync(transactionId), Is.Null);
		}

		[Test]
		public void DeleteTransactionById_ShouldThrowAnException_WithInvalidInput()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await transactionService.DeleteTransaction(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task EditTransactionFormModelById_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionFormModel? transactionFormModel = await transactionService
				.GetFulfilledTransactionFormModel(this.Transaction1.Id);

			//Assert
			Assert.That(transactionFormModel, Is.Not.Null);
			//Assert.That(transactionFormModel.Id, Is.EqualTo(this.Transaction1.Id));
			Assert.That(transactionFormModel.AccountId, Is.EqualTo(this.Transaction1.AccountId));
			Assert.That(transactionFormModel.Amount, Is.EqualTo(this.Transaction1.Amount));
			Assert.That(transactionFormModel.CategoryId, Is.EqualTo(this.Transaction1.CategoryId));
			Assert.That(transactionFormModel.Refference, Is.EqualTo(this.Transaction1.Refference));
			Assert.That(transactionFormModel.TransactionType, Is.EqualTo(this.Transaction1.TransactionType));
		}

		[Test]
		public void EditTransactionFormModelById_ShouldReturnNull_WithInValidInput()
		{
			//Act & Assert
			Assert.That(async () => await transactionService.GetFulfilledTransactionFormModel(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task TransactionViewModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Act
			TransactionDetailsViewModel transactionFormModel = 
				await transactionService.GetTransactionViewModel(this.Transaction1.Id);

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
		public void TransactionViewModel_ShouldReturnNull_WithInValidInput()
		{
			//Act & Assert
			Assert.That(async () => await transactionService.GetTransactionViewModel(Guid.NewGuid().ToString()),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task AllTransactionsViewModel_ShouldReturnCorrectDTO_WithValidInput()
		{
			//Arrange
			var model = new UserTransactionsViewModel
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
			await transactionService.GetAllUserTransactions(this.User1.Id, model);

			//Assert
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Transactions.Count(), Is.EqualTo(expectedTransactions.Count()));
			for (int i = 0; i < expectedTransactions.Count(); i++)
			{
				Assert.That(model.Transactions.ElementAt(i).Id,
					Is.EqualTo(expectedTransactions.ElementAt(i).Id));
				Assert.That(model.Transactions.ElementAt(i).Amount,
					Is.EqualTo(expectedTransactions.ElementAt(i).Amount));
				Assert.That(model.Transactions.ElementAt(i).CategoryName,
					Is.EqualTo(expectedTransactions.ElementAt(i).Category.Name));
				Assert.That(model.Transactions.ElementAt(i).Refference,
					Is.EqualTo(expectedTransactions.ElementAt(i).Refference));
				Assert.That(model.Transactions.ElementAt(i).TransactionType,
					Is.EqualTo(expectedTransactions.ElementAt(i).TransactionType.ToString()));
			}
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalance_WhenTransactionTypeIsChanged()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
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

			TransactionFormModel transactionEditModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<TransactionFormModel>(t))
				.FirstAsync();

			//Act
			transactionEditModel.TransactionType = TransactionType.Expense;
			await transactionService.EditTransaction(transactionId.ToString(), transactionEditModel);

			//Assert
			Assert.That(transaction.TransactionType, Is.EqualTo(TransactionType.Expense));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore - transaction.Amount * 2));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransactionAndChangeBalanceOnTwoAccounts_WhenAccountIsChanged()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
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
			TransactionFormModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<TransactionFormModel>(t))
				.FirstAsync();

			editTransactionModel.AccountId = this.Account1User1.Id;
			await transactionService.EditTransaction(transactionId.ToString(), editTransactionModel);

			//Assert
			Assert.That(transaction.AccountId, Is.EqualTo(this.Account1User1.Id));
			Assert.That(this.Account2User1.Balance, Is.EqualTo(firstAccBalanceBefore - transaction.Amount));
			Assert.That(this.Account1User1.Balance, Is.EqualTo(secondAccBalanceBefore + transaction.Amount));
		}

		[Test]
		public async Task EditTransaction_ShouldEditTransaction_WhenPaymentRefferenceIsChanged()
		{
			//Arrange
			string transactionId = Guid.NewGuid().ToString();
			Transaction transaction = new Transaction
			{
				Id = transactionId,
				OwnerId = this.User1.Id,
				AccountId = this.Account1User1.Id,
				Amount = 123,
				CategoryId = this.Category2.Id,
				CreatedOn = DateTime.UtcNow,
				Refference = "First Refference",
				TransactionType = TransactionType.Income
			};

			await data.Transactions.AddAsync(transaction);
			this.Account1User1.Balance += transaction.Amount;
			await data.SaveChangesAsync();
			decimal balanceBefore = this.Account1User1.Balance;
			string categoryIdBefore = transaction.CategoryId;

			//Act
			TransactionFormModel editTransactionModel = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => mapper.Map<TransactionFormModel>(t))
				.FirstAsync();
			editTransactionModel.Refference = "Second Refference";
			await transactionService.EditTransaction(transactionId.ToString(), editTransactionModel);

			//Assert that only transaction refference is changed
			Assert.That(this.Account1User1.Balance, Is.EqualTo(balanceBefore));
			Assert.That(transaction.Refference, Is.EqualTo(editTransactionModel.Refference));
			Assert.That(transaction.CategoryId, Is.EqualTo(categoryIdBefore));
		}
	}
}
