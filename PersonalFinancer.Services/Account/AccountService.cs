namespace PersonalFinancer.Services.Account
{
	using Microsoft.EntityFrameworkCore;
	using AutoMapper;
	using AutoMapper.QueryableExtensions;

	using Models;
	using Data;
	using Data.Enums;
	using Data.Models;
	using static Data.DataConstants.Category;
	using Services.Infrastructure;

	public class AccountService : IAccountService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;

		public AccountService(
			PersonalFinancerDbContext context,
			IMapper mapper)
		{
			this.data = context;
			this.mapper = mapper;
		}

		/// <summary>
		/// Returns User's accounts with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountDropdownViewModel>> AllAccountsDropdownViewModel(string userId)
		{
			IEnumerable<AccountDropdownViewModel> accounts = await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.ToArrayAsync();

			return accounts;
		}

		/// <summary>
		/// Returns Account with Id and Name, or throws an error.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<AccountDropdownViewModel> AccountDropdownViewModel(Guid accountId)
		{
			AccountDropdownViewModel account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.FirstAsync();

			return account;
		}

		/// <summary>
		/// Returns Account with Id, Name, Balance and all Transactions, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<AccountDetailsViewModel> AccountDetailsViewModel(Guid accountId)
		{
			var account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<AccountDetailsViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
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
				.Select(a => mapper.Map<AccountTypeViewModel>(a))
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
					CreatedOn = DateTime.UtcNow,
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
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		public async Task DashboardViewModel(string userId, DashboardServiceModel model)
		{
			model.LastTransactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.ProjectTo<TransactionShortViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.ToArrayAsync();

			model.Accounts = await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.ProjectTo<AccountCardViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.ToArrayAsync();

			await data.Accounts
				.Where(a => a.OwnerId == userId && a.Transactions.Any())
				.Include(a => a.Currency)
				.Include(a => a.Transactions
					.Where(t => t.CreatedOn >= model.StartDate && t.CreatedOn <= model.EndDate))
				.ForEachAsync(a =>
				{
					if (!model.CurrenciesCashFlow.ContainsKey(a.Currency.Name))
					{
						model.CurrenciesCashFlow[a.Currency.Name] = new CashFlowViewModel();
					}

					var income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						model.CurrenciesCashFlow[a.Currency.Name].Income += (decimal)income;
					}

					var expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						model.CurrenciesCashFlow[a.Currency.Name].Expence += (decimal)expense;
					}
				});
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
				.ProjectTo<TransactionFormModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.FirstAsync();

			return transaction;
		}

		/// <summary>
		/// Returns Transaction Extended View Model with given Id, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<TransactionExtendedViewModel> TransactionViewModel(Guid transactionId)
		{
			var transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionExtendedViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.FirstAsync();

			return transaction;
		}

		/// <summary>
		/// Returns a collection of User's transactions for given period.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		public async Task<AllTransactionsServiceModel> AllTransactionsViewModel(
			string userId, AllTransactionsServiceModel model)
		{
			var transactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.ProjectTo<TransactionExtendedViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.ToArrayAsync();

			model.Transactions = transactions;

			return model;
		}

		/// <summary>
		/// Edits a Transaction and change account's balance if it's nessesery, or throws an exception.
		/// </summary>
		/// <param name="editedTransaction">
		/// Model with props: Id, AccountId, TransactionType, Amount, Refference, CategoryId, CreatedOn.
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
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
	}
}
