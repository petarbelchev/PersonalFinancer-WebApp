namespace PersonalFinancer.Services.Account
{
	using Microsoft.EntityFrameworkCore;

	using Data;
	using Data.Enums;
	using Data.Models;
	using Models;
	using static Data.DataConstants.Category;

	public class AccountService : IAccountService
	{
		private readonly PersonalFinancerDbContext data;

		public AccountService(PersonalFinancerDbContext context)
		{
			this.data = context;
		}

		public async Task<IEnumerable<AccountViewModel>> AllAccounts(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId)
				.Select(a => new AccountViewModel
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToArrayAsync();
		}

		public async Task<IEnumerable<AccountViewModelExtended>> AllAccountsWithData(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId)
				.Select(a => new AccountViewModelExtended
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					Currency = a.Currency.Name
				})
				.ToArrayAsync();
		}

		public async Task<IEnumerable<AccountTypeViewModel>> AllAccountTypes(string userId)
		{
			return await data.AccountTypes
				.Where(a => a.UserId == null || a.UserId == userId)
				.Select(a => new AccountTypeViewModel
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToArrayAsync();
		}

		public async Task ChangeBalance(int accountId, decimal amount, TransactionType transactionType)
		{
			var account = await data.Accounts.FirstAsync(a => a.Id == accountId);

			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		public async Task CreateAccount(string userId, CreateAccountFormModel accountModel)
		{
			var newAccount = new Account()
			{
				Name = accountModel.Name,
				Balance = accountModel.Balance,
				AccountTypeId = accountModel.AccountTypeId,
				CurrencyId = accountModel.CurrencyId,
				OwnerId = userId
			};

			await data.Accounts.AddAsync(newAccount);

			if (newAccount.Balance != 0)
			{
				await data.Transactions.AddAsync(new Transaction()
				{
					Amount = newAccount.Balance,
					Account = newAccount,
					CategoryId = 1, // Id for initial balance 
					Refference = "Initial Balance",
					CreatedOn = DateTime.Now,
					TransactionType = TransactionType.Income
				});
			}

			await data.SaveChangesAsync();
		}

		public async Task<bool> IsAccountOwner(string userId, int accountId)
		{
			var account = await data.Accounts.FindAsync(accountId);

			if (account == null)
				return false;

			return account.OwnerId == userId;
		}

		public async Task CreateTransaction(TransactionServiceModel transactionFormModel)
		{
			var newTransaction = new Transaction()
			{
				Amount = transactionFormModel.Amount,
				AccountId = transactionFormModel.AccountId,
				CategoryId = transactionFormModel.CategoryId,
				TransactionType = transactionFormModel.TransactionType,
				CreatedOn = transactionFormModel.CreatedOn,
				Refference = transactionFormModel.Refference
			};

			await ChangeBalance(newTransaction.AccountId,
								newTransaction.Amount,
								newTransaction.TransactionType);

			await data.Transactions.AddAsync(newTransaction);

			await data.SaveChangesAsync();
		}

		public async Task<TransactionServiceModel?> GetTransactionById(int id)
		{
			var transaction = await data.Transactions
				.Where(t => t.Id == id)
				.Select(t => new TransactionServiceModel
				{
					Id = t.Id,
					AccountId = t.AccountId,
					Amount = t.Amount,
					CategoryId = t.CategoryId,
					Refference = t.Refference,
					TransactionType = t.TransactionType,
					OwnerId = t.Account.OwnerId,
					CreatedOn = t.CreatedOn
				})
				.FirstOrDefaultAsync();

			if (transaction == null)
				return null;

			return transaction;
		}

		public async Task<IEnumerable<TransactionViewModel>> GetTransactionsByAccountId(int id)
		{
			return await data.Transactions
				.Where(t => t.AccountId == id)
				.Select(t => new TransactionViewModel
				{
					Id = t.Id,
					Amount = t.Amount,
					Category = t.Category.Name,
					CreatedOn = t.CreatedOn,
					Refference = t.Refference,
					TransactionType = t.TransactionType.ToString()
				})
				.ToArrayAsync();
		}

		public async Task<IEnumerable<TransactionViewModel>> LastFiveTransactions(string userId)
		{
			return await data.Transactions
				.Where(t => t.Account.OwnerId == userId)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.Select(t => new TransactionViewModel
				{
					Id = t.Id,
					Account = t.Account.Name,
					Currency = t.Account.Currency.Name,
					Amount = t.Amount,
					TransactionType = t.TransactionType.ToString(),
					Category = t.Category.Name,
					CreatedOn = t.CreatedOn,
					Refference = t.Refference
				})
				.ToArrayAsync();
		}

		public async Task DeleteTransactionById(int id)
		{
			var transaction = await data.Transactions
				.Where(t => t.Id == id)
				.Include(t => t.Category)
				.FirstAsync();

			data.Transactions.Remove(transaction);

			if (transaction.Category.Name == CategoryInitialBalanceName)
				transaction.TransactionType = TransactionType.Expense;

			await ChangeBalance(transaction.AccountId,
								transaction.Amount,
								transaction.TransactionType);

			await data.SaveChangesAsync();
		}

		public async Task EditTransaction(TransactionServiceModel editedTransaction)
		{
			var transaction = await data.Transactions
				.FirstAsync(t => t.Id == editedTransaction.Id);

			bool isAccountOrAmountOrTransactionTypeChanged =
				editedTransaction.AccountId != transaction.AccountId ||
				editedTransaction.TransactionType != transaction.TransactionType ||
				editedTransaction.Amount != transaction.Amount;

			if (isAccountOrAmountOrTransactionTypeChanged)
			{
				var newTransactionType = TransactionType.Income;

				if (transaction.TransactionType == TransactionType.Income)
					newTransactionType = TransactionType.Expense;

				await ChangeBalance(transaction.AccountId,
									transaction.Amount,
									newTransactionType);
			}

			transaction.Refference = editedTransaction.Refference;
			transaction.AccountId = editedTransaction.AccountId;
			transaction.CategoryId = editedTransaction.CategoryId;
			transaction.Amount = editedTransaction.Amount;
			transaction.CreatedOn = editedTransaction.CreatedOn;
			transaction.TransactionType = editedTransaction.TransactionType;

			if (isAccountOrAmountOrTransactionTypeChanged)
			{
				await ChangeBalance(transaction.AccountId,
									transaction.Amount,
									transaction.TransactionType);
			}

			await data.SaveChangesAsync();
		}

		public async Task<AccountViewModelExtended> GetAccountById(int id)
		{
			var account = await data.Accounts
				.Where(a => a.Id == id)
				.Select(a => new AccountViewModelExtended
				{
					Id = a.Id,
					Balance = a.Balance,
					Name = a.Name,
					Currency = a.Currency.Name
				})
				.FirstAsync();

			account.Transactions = await GetTransactionsByAccountId(account.Id);

			return account;
		}
	}
}
