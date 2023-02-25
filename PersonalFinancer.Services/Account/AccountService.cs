namespace PersonalFinancer.Services.Account
{
	using Microsoft.EntityFrameworkCore;
	using AutoMapper;
	using AutoMapper.QueryableExtensions;

	using Models;
	using Infrastructure;
	using Data;
	using Data.Enums;
	using Data.Models;
	using static Data.DataConstants.Category;

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
		/// Returns collection of User's accounts with Id and Name.
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
		/// Returns Account with Id and Name or null.
		/// </summary>
		public async Task<AccountDropdownViewModel?> AccountDropdownViewModel(Guid accountId)
		{
			AccountDropdownViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.FirstOrDefaultAsync();

			return account;
		}

		/// <summary>
		/// Returns Account with Id, Name, Balance and all Transactions or null.
		/// </summary>
		public async Task<AccountDetailsViewModel?> AccountDetailsViewModel(Guid accountId)
		{
			AccountDetailsViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<AccountDetailsViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.FirstOrDefaultAsync();

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
		/// Changes balance on given Account or throws exception if Account does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task ChangeBalance(Guid accountId, decimal amount, TransactionType transactionType)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

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
		/// Returns new Account's id.
		/// </summary>
		/// <param name="userId">User's identifier</param>
		/// <param name="accountModel">Model with Name, Balance, AccountTypeId, CurrencyId.</param>
		public async Task<Guid> CreateAccount(string userId, AccountFormModel accountModel)
		{
			Account newAccount = new Account()
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

			return newAccount.Id;
		}

		/// <summary>
		/// Checks is the given User is owner of the given account, if does not exist, throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<bool> IsAccountOwner(string userId, Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.OwnerId == userId;
		}

		/// <summary>
		/// Checks is the given Account deleted, if does not exist, throws an exception.
		/// </summary>
		/// <param name="accountId"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<bool> IsAccountDeleted(Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.IsDeleted;
		}

		/// <summary>
		/// Creates a new Transaction and change account's balance. Returns new transaction's id.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<Guid> CreateTransaction(TransactionFormModel transactionFormModel)
		{
			Transaction newTransaction = new Transaction()
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

			return newTransaction.Id;
		}

		/// <summary>
		/// Delete a Transaction and change account's balance. Returns True or False.
		/// </summary>
		public async Task<bool> DeleteTransactionById(Guid transactionId)
		{
			Transaction? transaction = await data.Transactions.FindAsync(transactionId);

			if (transaction == null)
			{
				return false;
			}

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

			return true;
		}

		/// <summary>
		/// Delete an Account and give the option to delete all of the account's transactions.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task DeleteAccountById(Guid accountId, bool shouldDeleteTransactions)
		{
			Account? account = await data.Accounts
				.FirstOrDefaultAsync(a => a.Id == accountId && !a.IsDeleted);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

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
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		public async Task DashboardViewModel(string userId, DashboardServiceModel model)
		{
			model.Accounts = await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.ProjectTo<AccountCardViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.ToArrayAsync();

			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

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

					decimal? income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						model.CurrenciesCashFlow[a.Currency.Name].Income += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						model.CurrenciesCashFlow[a.Currency.Name].Expence += (decimal)expense;
					}
				});
		}

		/// <summary>
		/// Returns a Transaction with Id, AccountId, Amount, CategoryId, Refference, TransactionType, OwnerId, CreatedOn, or null.
		/// </summary>
		public async Task<EditTransactionFormModel?> EditTransactionFormModelById(Guid transactionId)
		{
			EditTransactionFormModel? transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<EditTransactionFormModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.FirstOrDefaultAsync();

			return transaction;
		}

		/// <summary>
		/// Returns Transaction Extended View Model with given Id, or null.
		/// </summary>
		public async Task<TransactionExtendedViewModel?> TransactionViewModel(Guid transactionId)
		{
			TransactionExtendedViewModel? transaction = await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionExtendedViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.FirstOrDefaultAsync();

			return transaction;
		}

		/// <summary>
		/// Returns a collection of User's transactions for given period. 
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		public async Task<AllTransactionsServiceModel> AllTransactionsViewModel(
			string userId, AllTransactionsServiceModel model)
		{
			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			TransactionExtendedViewModel[] transactions = await data.Transactions
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
		/// <exception cref="ArgumentNullException"></exception>
		public async Task EditTransaction(EditTransactionFormModel editedTransaction)
		{
			Transaction? transaction = await data.Transactions
				.FindAsync(editedTransaction.Id);

			if (transaction == null)
			{
				throw new ArgumentNullException("Transactions does not exist.");
			}

			bool isAccountOrAmountOrTransactionTypeChanged =
				editedTransaction.AccountId != transaction.AccountId ||
				editedTransaction.TransactionType != transaction.TransactionType ||
				editedTransaction.Amount != transaction.Amount;

			if (isAccountOrAmountOrTransactionTypeChanged)
			{
				TransactionType newTransactionType = TransactionType.Income;

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
