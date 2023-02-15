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
			=> this.data = context;

		/// <summary>
		/// Returns User's accounts with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountDropdownViewModel>> AccountsByUserId(string userId)
		{
			IEnumerable<AccountDropdownViewModel> accounts =  await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.Select(a => new AccountDropdownViewModel
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToArrayAsync();

			return accounts;
		}

		/// <summary>
		/// Returns Account with Id and Name, or throws an error.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<AccountDropdownViewModel> AccountById(Guid accountId)
		{
			AccountDropdownViewModel account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.Select(a => new AccountDropdownViewModel
				{
					Id = a.Id,
					Name = a.Name
				})
				.FirstAsync();

			return account;
		}

		/// <summary>
		/// Returns Account with Id, Name, Balance and all Transactions, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<AccountDetailsViewModel> AccountWithTransactions(Guid accountId)
		{
			var account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.Select(a => new AccountDetailsViewModel
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					Currency = a.Currency.Name,
					Transactions = a.Transactions
						.Select(t => new TransactionExtendedViewModel
						{
							Id = t.Id,
							Amount = t.Amount,
							Category = t.Category.Name,
							CreatedOn = t.CreatedOn,
							Refference = t.Refference,
							TransactionType = t.TransactionType.ToString()
						})
				})
				.FirstAsync();

			return account;
		}

		/// <summary>
		/// Returns collection of Account Types for the current user with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountTypeViewModel>> AccountTypesViewModel(string userId)
		{
			return await data.AccountTypes
				.Where(a => (a.UserId == null || a.UserId == userId) && !a.IsDeleted)
				.Select(a => new AccountTypeViewModel
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToArrayAsync();
		}

		/// <summary>
		/// Changes balance on given Account, or throws an exception.
		/// </summary>
		/// <param name="accountId">Account's identifier.</param>
		/// <param name="amount">Amount that will be applied to balance.</param>
		/// <param name="transactionType">Defines what will be the change on the balance.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task ChangeBalance(Guid accountId, decimal amount, TransactionType transactionType)
		{
			var account = await data.Accounts.FirstAsync(a => a.Id == accountId);

			if (transactionType == TransactionType.Income)
			{
				account.Balance += amount;
			}
			else if (transactionType == TransactionType.Expense)
			{
				account.Balance -= amount;
			}
		}

		/// <summary>
		/// Creates a new Account and if the new account has initial balance creates new Transaction with given amount.
		/// </summary>
		/// <param name="userId">User's identifier</param>
		/// <param name="accountModel">Model with Name, Balance, AccountTypeId, CurrencyId.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="DbUpdateException"></exception>
		/// <exception cref="DbUpdateConcurrencyException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task CreateAccount(string userId, AccountFormModel accountModel)
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
					Category = await data.Categories.FirstAsync(c => c.Name == CategoryInitialBalanceName),
					Refference = "Initial Balance",
					CreatedOn = DateTime.Now,
					TransactionType = TransactionType.Income
				});
			}

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Checks is the given User is owner of the given account
		/// </summary>
		public async Task<bool> IsAccountOwner(string userId, Guid accountId)
		{
			var account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				return false;
			}

			return account.OwnerId == userId;
		}

		/// <summary>
		/// Creates a new Transaction and change account's balance.
		/// </summary>
		/// <param name="transactionFormModel">
		/// Model with Amount, AccountId, CategoryId, TransactionType, CreatedOn, Refference.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="DbUpdateException"></exception>
		/// <exception cref="DbUpdateConcurrencyException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task CreateTransaction(TransactionFormModel transactionFormModel)
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

		/// <summary>
		/// Delete a Transaction and change account's balance, or throws an exception.
		/// </summary>
		/// <param name="id">Transaction's identifier.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="DbUpdateException"></exception>
		/// <exception cref="DbUpdateConcurrencyException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task DeleteTransactionById(Guid transactionId)
		{
			var transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Include(t => t.Category)
				.FirstAsync();

			data.Transactions.Remove(transaction);

			if (transaction.TransactionType == TransactionType.Income)
			{
				transaction.TransactionType = TransactionType.Expense;
			}
			else
			{
				transaction.TransactionType = TransactionType.Income;
			}

			await ChangeBalance(transaction.AccountId,
								transaction.Amount,
								transaction.TransactionType);

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Delete an Account and give the option to delete all of the account's transactions.
		/// </summary>
		/// <param name="accountId">Account's identifier.</param>
		/// <param name="shouldDeleteTransactions">
		/// Boolean that defines that should delete account's transactions.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="DbUpdateException"></exception>
		/// <exception cref="DbUpdateConcurrencyException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task DeleteAccountById(Guid accountId, bool shouldDeleteTransactions)
		{
			var account = await data.Accounts.FirstAsync(a => a.Id == accountId && !a.IsDeleted);

			if (shouldDeleteTransactions)
			{
				data.Accounts.Remove(account);
			}
			else
			{
				account.IsDeleted = true;
			}

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Returns Dashboard View Model for current User with Last transactions, Accounts and Currencies Cash Flow.
		/// </summary>
		public async Task<DashboardViewModel> DashboardViewModel(string userId)
		{
			return new DashboardViewModel
			{
				LastTransactions = await LastFiveTransactionsByUserId(userId),
				Accounts = await AccountsWithBalance(userId),
				CurrenciesCashFlow = await GetCashFlow(userId)
			};
		}

		/// <summary>
		/// Returns a Transaction with Id, AccountId, Amount, CategoryId, Refference, TransactionType, OwnerId, CreatedOn, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<TransactionFormModel> TransactionFormModelById(Guid transactionId)
		{
			var transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => new TransactionFormModel
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
				.FirstAsync();

			return transaction;
		}

		/// <summary>
		/// Returns Transaction Extended View Model with given Id, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<TransactionExtendedViewModel> TransactionViewModelById(Guid transactionId)
		{
			var transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.Select(t => new TransactionExtendedViewModel
				{
					Id = t.Id,
					Account = t.Account.Name,
					Amount = t.Amount,
					Currency = t.Account.Currency.Name,
					Category = t.Category.Name,
					Refference = t.Refference,
					TransactionType = t.TransactionType.ToString(),
					CreatedOn = t.CreatedOn
				})
				.FirstAsync();

			return transaction;
		}

		/// <summary>
		/// Returns a collection of User's transactions with props: 
		/// Id, AccountName, Amount, CurrencyName, CategoryName, Refference, TransactionType, CreatedOn.
		/// </summary>
		public async Task<IEnumerable<TransactionExtendedViewModel>> TransactionsViewModelByUserId(string userId)
		{
			var transactions = await data.Transactions
				.Where(t => t.Account.OwnerId == userId)
				.OrderByDescending(t => t.CreatedOn)
				.Select(t => new TransactionExtendedViewModel
				{
					Id = t.Id,
					Account = t.Account.Name,
					Amount = t.Amount,
					Currency = t.Account.Currency.Name,
					Category = t.Category.Name,
					Refference = t.Refference,
					TransactionType = t.TransactionType.ToString(),
					CreatedOn = t.CreatedOn
				})
				.ToArrayAsync();

			return transactions;
		}

		/// <summary>
		/// Edits a Transaction and change account's balance if it's nessesery, or throws an exception.
		/// </summary>
		/// <param name="editedTransaction">
		/// Model with props: Id, AccountId, TransactionType, Amount, Refference, CategoryId, CreatedOn.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="DbUpdateException"></exception>
		/// <exception cref="DbUpdateConcurrencyException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task EditTransaction(TransactionFormModel editedTransaction)
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
				{
					newTransactionType = TransactionType.Expense;
				}

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

		/// <summary>
		/// Returns collection of User's accounts with props: Id, Name, Balance, Currency.
		/// </summary>
		private async Task<IEnumerable<AccountCardViewModel>> AccountsWithBalance(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.Select(a => new AccountCardViewModel
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					Currency = a.Currency.Name
				})
				.ToArrayAsync();
		}

		/// <summary>
		/// Returns collection of last five User's transactions with props: Id, Account, Currency, Amount, Transaction Type, CreatedOn.
		/// </summary>
		private async Task<IEnumerable<TransactionShortViewModel>> LastFiveTransactionsByUserId(string userId)
		{
			return await data.Transactions
				.Where(t => t.Account.OwnerId == userId)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.Select(t => new TransactionShortViewModel
				{
					Id = t.Id,
					Account = t.Account.Name + (t.Account.IsDeleted ? " (Deleted)" : string.Empty),
					Currency = t.Account.Currency.Name,
					Amount = t.Amount,
					TransactionType = t.TransactionType.ToString(),
					CreatedOn = t.CreatedOn
				})
				.ToArrayAsync();
		}

		/// <summary>
		/// Returns User account's cash flow
		/// </summary>
		/// <returns>Dictionary which key is Currency and value with props: Income and Expense.</returns>
		private async Task<Dictionary<string, CashFlowViewModel>> GetCashFlow(string userId)
		{
			var cashFlow = new Dictionary<string, CashFlowViewModel>();

			await data.Accounts
				.Where(a => a.OwnerId == userId && a.Transactions.Any())
				.Include(a => a.Currency)
				.Include(a => a.Transactions)
				.ForEachAsync(a =>
				{
					if (!cashFlow.ContainsKey(a.Currency.Name))
					{
						cashFlow[a.Currency.Name] = new CashFlowViewModel();
					}

					var income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						cashFlow[a.Currency.Name].Income += (decimal)income;
					}

					var expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						cashFlow[a.Currency.Name].Expence += (decimal)expense;
					}
				});

			return cashFlow;
		}
	}
}
