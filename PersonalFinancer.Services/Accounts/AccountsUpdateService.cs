namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using static PersonalFinancer.Data.Constants;

	public class AccountsUpdateService : IAccountsUpdateService
	{
		private readonly IEfRepository<Account> accountsRepo;
		private readonly IEfRepository<Transaction> transactionsRepo;
		private readonly IEfRepository<AccountType> accountTypesRepo;
		private readonly IEfRepository<Currency> currenciesRepo;
		private readonly IEfRepository<Category> categoriesRepo;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public AccountsUpdateService(
			IEfRepository<Account> accountRepository,
			IEfRepository<Transaction> transactionRepository,
			IEfRepository<AccountType> accountTypesRepo,
			IEfRepository<Currency> currenciesRepo,
			IEfRepository<Category> categoriesRepo,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.accountsRepo = accountRepository;
			this.transactionsRepo = transactionRepository;
			this.accountTypesRepo = accountTypesRepo;
			this.currenciesRepo = currenciesRepo;
			this.categoriesRepo = categoriesRepo;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		/// <summary>
		/// Throws ArgumentException when User already have Account with the given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<Guid> CreateAccountAsync(AccountFormShortServiceModel model)
		{
			if (await this.IsNameExistAsync(model.Name, model.OwnerId))
				throw new ArgumentException($"The User already have Account with \"{model.Name}\" name.");

			await this.ValidateAccountTypeAndCurrency(model);

			Account newAccount = this.mapper.Map<Account>(model);
			newAccount.Id = Guid.NewGuid();

			if (newAccount.Balance != 0)
			{
				Transaction initialTransaction =
					CreateInitialTransaction(newAccount.Id, newAccount.OwnerId, newAccount.Balance);

				newAccount.Transactions.Add(initialTransaction);
			}

			await this.accountsRepo.AddAsync(newAccount);
			await this.accountsRepo.SaveChangesAsync();

			this.memoryCache.Remove(AccountConstants.AccountCacheKeyValue + model.OwnerId);

			return newAccount.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <returns>New transaction Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> CreateTransactionAsync(TransactionFormModel model)
		{
			Account account = await this.accountsRepo.All()
				.FirstAsync(a => a.Id == model.AccountId);

			await this.ValidateCategory(
				model.CategoryId ?? throw new InvalidOperationException("Category ID cannot be a null."), 
				model.OwnerId ?? throw new InvalidOperationException("Owner ID cannot be a null."));

			model.CreatedOn = model.CreatedOn.ToUniversalTime();

			Transaction newTransaction = this.mapper.Map<Transaction>(model);
			newTransaction.Id = Guid.NewGuid();

			await this.transactionsRepo.AddAsync(newTransaction);

			ChangeBalance(account, newTransaction.Amount, model.TransactionType);

			await this.accountsRepo.SaveChangesAsync();

			return newTransaction.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccountAsync(
			Guid accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false)
		{
			Account account = await this.accountsRepo.All()
				.FirstAsync(a => a.Id == accountId && !a.IsDeleted);

			if (!isUserAdmin && account.OwnerId != userId)
				throw new ArgumentException("Can't delete someone else account.");

			if (shouldDeleteTransactions)
				this.accountsRepo.Remove(account);
			else
				account.IsDeleted = true;

			await this.accountsRepo.SaveChangesAsync();

			this.memoryCache.Remove(AccountConstants.AccountCacheKeyValue + userId);
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<decimal> DeleteTransactionAsync(Guid transactionId, Guid userId, bool isUserAdmin)
		{
			Transaction transaction = await this.transactionsRepo.All()
			   .Include(t => t.Account)
			   .FirstAsync(t => t.Id == transactionId);

			if (!isUserAdmin && transaction.OwnerId != userId)
				throw new ArgumentException("User is not transaction's owner");

			this.transactionsRepo.Remove(transaction);

			RefundBalance(transaction);

			await this.transactionsRepo.SaveChangesAsync();

			return transaction.Account.Balance;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditAccountAsync(Guid accountId, AccountFormShortServiceModel model)
		{
			Account account = await this.accountsRepo.All()
				.FirstAsync(a => a.Id == accountId);

			if (account.Name != model.Name && await this.IsNameExistAsync(model.Name, model.OwnerId))
				throw new ArgumentException($"The User already have Account with \"{model.Name}\" name.");

			await this.ValidateAccountTypeAndCurrency(model);

			account.Name = model.Name.Trim();
			account.CurrencyId = model.CurrencyId;
			account.AccountTypeId = model.AccountTypeId;

			if (account.Balance != model.Balance)
			{
				decimal amountOfChange = model.Balance - account.Balance;
				account.Balance = model.Balance;

				Transaction? transaction = await this.transactionsRepo.All()
					.FirstOrDefaultAsync(t => t.AccountId == account.Id && t.IsInitialBalance);

				if (transaction == null)
				{
					Transaction initialTransaction =
						CreateInitialTransaction(account.Id, account.OwnerId, amountOfChange);

					await this.transactionsRepo.AddAsync(initialTransaction);
				}
				else
				{
					transaction.Amount += amountOfChange;

					if (transaction.Amount < 0)
						transaction.TransactionType = TransactionType.Expense;
				}
			}

			await this.accountsRepo.SaveChangesAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction, Category or Account does not exist
		/// or Transaction is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditTransactionAsync(Guid transactionId, TransactionFormModel model)
		{
			Transaction transactionInDb = await this.transactionsRepo.All()
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == transactionId);

			if (transactionInDb.IsInitialBalance)
				throw new InvalidOperationException("Cannot edit initial balance transaction.");

			Guid categoryId = model.CategoryId ?? throw new InvalidOperationException("Category Id cannot be a null.");

			await this.ValidateCategory(
				categoryId,	
				model.OwnerId ?? throw new InvalidOperationException("Owner ID cannot be a null."));

			if (model.AccountId != transactionInDb.AccountId
				|| model.TransactionType != transactionInDb.TransactionType
				|| model.Amount != transactionInDb.Amount)
			{
				RefundBalance(transactionInDb);

				if (model.AccountId != transactionInDb.AccountId)
				{
					Account newAccount = await this.accountsRepo.All()
						.FirstAsync(a => a.Id == model.AccountId);

					transactionInDb.Account = newAccount;
				}

				ChangeBalance(transactionInDb.Account, model.Amount, model.TransactionType);
			}

			transactionInDb.Reference = model.Reference.Trim();
			transactionInDb.AccountId = model.AccountId ?? throw new InvalidOperationException("Account ID cannot be a null!");
			transactionInDb.CategoryId = categoryId;
			transactionInDb.Amount = model.Amount;
			transactionInDb.CreatedOn = model.CreatedOn.ToUniversalTime();
			transactionInDb.TransactionType = model.TransactionType;

			await this.transactionsRepo.SaveChangesAsync();
		}

		private static Transaction CreateInitialTransaction(Guid accountId, Guid ownerId, decimal amount)
		{
			return new Transaction()
			{
				Id = Guid.NewGuid(),
				AccountId = accountId,
				OwnerId = ownerId,
				CategoryId = Guid.Parse(CategoryConstants.InitialBalanceCategoryId),
				Amount = amount,
				CreatedOn = DateTime.UtcNow,
				TransactionType = amount < 0
					? TransactionType.Expense
					: TransactionType.Income,
				Reference = CategoryConstants.CategoryInitialBalanceName,
				IsInitialBalance = true
			};
		}

		private static void ChangeBalance(Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		private async Task<bool> IsNameExistAsync(string name, Guid userId)
		{
			return await this.accountsRepo.All()
				.AnyAsync(a => a.OwnerId == userId && a.Name == name);
		}

		private static void RefundBalance(Transaction transaction)
		{
			ChangeBalance(
				transaction.Account,
				transaction.Amount,
				transaction.TransactionType == TransactionType.Income
					? TransactionType.Expense
					: TransactionType.Income);
		}

		private async Task ValidateAccountTypeAndCurrency(AccountFormShortServiceModel model)
		{
			bool isAccountTypeValid = await this.accountTypesRepo.All().AnyAsync(at =>
					at.Id == model.AccountTypeId
					&& at.OwnerId == model.OwnerId
					&& at.IsDeleted == false);

			if (!isAccountTypeValid)
				throw new InvalidOperationException("Account Type is not valid.");

			bool isCurrencyValid = await this.currenciesRepo.All().AnyAsync(c =>
					c.Id == model.CurrencyId
					&& c.OwnerId == model.OwnerId
					&& c.IsDeleted == false);

			if (!isCurrencyValid)
				throw new InvalidOperationException("Currency is not valid.");
		}

		private async Task ValidateCategory(Guid categoryId, Guid ownerId)
		{
			bool isCategoryValid = await this.categoriesRepo.All().AnyAsync(c =>
				c.Id == categoryId
				&& c.OwnerId == ownerId
				&& c.IsDeleted == false);

			if (!isCategoryValid)
				throw new InvalidOperationException("Category is not valid.");
		}
	}
}