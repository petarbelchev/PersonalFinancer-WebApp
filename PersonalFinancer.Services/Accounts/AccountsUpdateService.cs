namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using static PersonalFinancer.Common.Constants.CacheConstants;
	using static PersonalFinancer.Common.Constants.CategoryConstants;

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

		public async Task<Guid> CreateAccountAsync(CreateEditAccountInputDTO model)
		{
			if (await this.IsNameExistAsync(model.Name, model.OwnerId))
				throw new ArgumentException(string.Format(ExceptionMessages.ExistingUserEntityName, "account", model.Name));

			await this.ValidateAccountTypeAndCurrencyAsync(model);

			Account newAccount = this.mapper.Map<Account>(model);

			if (newAccount.Balance != 0)
			{
				Transaction initialTransaction =
					CreateInitialTransaction(newAccount.Id, newAccount.OwnerId, newAccount.Balance);

				newAccount.Transactions.Add(initialTransaction);
			}

			await this.accountsRepo.AddAsync(newAccount);
			await this.accountsRepo.SaveChangesAsync();
			this.memoryCache.Remove(AccountsAndCategoriesKey + newAccount.OwnerId);

			return newAccount.Id;
		}

		public async Task<Guid> CreateTransactionAsync(CreateEditTransactionInputDTO model)
		{
			Account account = await this.FindAccountAsync(model.AccountId);

			await this.ValidateCategoryAsync(model.CategoryId, model.OwnerId);

			Transaction newTransaction = this.mapper.Map<Transaction>(model);

			await this.transactionsRepo.AddAsync(newTransaction);

			ChangeAccountBalance(account, newTransaction.Amount, model.TransactionType);

			await this.accountsRepo.SaveChangesAsync();

			return newTransaction.Id;
		}

		public async Task DeleteAccountAsync(
			Guid accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false)
		{
			Account account = await this.FindAccountAsync(accountId);

			if (!isUserAdmin && account.OwnerId != userId)
				throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);

			if (shouldDeleteTransactions)
				this.accountsRepo.Remove(account);
			else
				account.IsDeleted = true;

			await this.accountsRepo.SaveChangesAsync();
			this.memoryCache.Remove(AccountsAndCategoriesKey + account.OwnerId);
		}

		public async Task<decimal> DeleteTransactionAsync(
			Guid transactionId, Guid userId, bool isUserAdmin)
		{
			Transaction transaction = await this.transactionsRepo.All()
			   .Include(t => t.Account)
			   .FirstAsync(t => t.Id == transactionId);

			if (!isUserAdmin && transaction.OwnerId != userId)
				throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);

			this.transactionsRepo.Remove(transaction);

			RestoreAccountBalance(transaction);

			await this.transactionsRepo.SaveChangesAsync();

			return transaction.Account.Balance;
		}

		public async Task EditAccountAsync(Guid accountId, CreateEditAccountInputDTO model)
		{
			Account account = await this.FindAccountAsync(accountId);

			if (account.Name != model.Name && await this.IsNameExistAsync(model.Name, model.OwnerId))
				throw new ArgumentException(string.Format(ExceptionMessages.ExistingUserEntityName, "account", model.Name));

			await this.ValidateAccountTypeAndCurrencyAsync(model);

			account.Name = model.Name.Trim();
			account.CurrencyId = model.CurrencyId;
			account.AccountTypeId = model.AccountTypeId;

			if (account.Balance != model.Balance)
			{
				decimal balanceChange = model.Balance - account.Balance;
				account.Balance = model.Balance;

				Transaction? transaction = await this.transactionsRepo.All()
					.FirstOrDefaultAsync(t => t.AccountId == account.Id && t.IsInitialBalance);

				if (transaction == null)
				{
					Transaction initialTransaction =
						CreateInitialTransaction(account.Id, account.OwnerId, balanceChange);

					await this.transactionsRepo.AddAsync(initialTransaction);
				}
				else
				{
					transaction.Amount += balanceChange;

					if (transaction.Amount < 0)
						transaction.TransactionType = TransactionType.Expense;
				}
			}

			await this.accountsRepo.SaveChangesAsync();
		}

		public async Task EditTransactionAsync(Guid transactionId, CreateEditTransactionInputDTO model)
		{
			Transaction transactionInDb = await this.transactionsRepo.All()
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == transactionId);

			if (transactionInDb.IsInitialBalance)
				throw new InvalidOperationException(ExceptionMessages.EditInitialTransaction);

			await this.ValidateCategoryAsync(model.CategoryId, model.OwnerId);

			bool isNeedBalanceChange =
				model.AccountId != transactionInDb.AccountId ||
				model.TransactionType != transactionInDb.TransactionType ||
				model.Amount != transactionInDb.Amount;

			if (isNeedBalanceChange)
			{
				RestoreAccountBalance(transactionInDb);

				if (model.AccountId != transactionInDb.AccountId)
				{
					Account newAccount = await this.FindAccountAsync(model.AccountId);
					transactionInDb.Account = newAccount;
				}

				transactionInDb.Amount = model.Amount;
				transactionInDb.TransactionType = model.TransactionType;

				ChangeAccountBalance(transactionInDb.Account, transactionInDb.Amount, transactionInDb.TransactionType);
			}

			transactionInDb.Reference = model.Reference.Trim();
			transactionInDb.CategoryId = model.CategoryId;
			transactionInDb.CreatedOnUtc = model.CreatedOnLocalTime.ToUniversalTime();

			await this.transactionsRepo.SaveChangesAsync();
		}

		private static void ChangeAccountBalance(
			Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		private static Transaction CreateInitialTransaction(Guid accountId, Guid ownerId, decimal amount)
		{
			return new Transaction()
			{
				AccountId = accountId,
				OwnerId = ownerId,
				CategoryId = Guid.Parse(InitialBalanceCategoryId),
				Amount = amount,
				CreatedOnUtc = DateTime.UtcNow,
				TransactionType = amount < 0
					? TransactionType.Expense
					: TransactionType.Income,
				Reference = CategoryInitialBalanceName,
				IsInitialBalance = true
			};
		}

		/// <exception cref="InvalidOperationException">When the account does not exist.</exception>
		private async Task<Account> FindAccountAsync(Guid accountId)
			=> await this.accountsRepo.All().FirstAsync(a => a.Id == accountId && !a.IsDeleted);

		private async Task<bool> IsNameExistAsync(string name, Guid userId)
			=> await this.accountsRepo.All().AnyAsync(a => a.OwnerId == userId && a.Name == name.Trim());

		private static void RestoreAccountBalance(Transaction transaction)
		{
			ChangeAccountBalance(
				transaction.Account,
				transaction.Amount,
				transaction.TransactionType == TransactionType.Income
					? TransactionType.Expense
					: TransactionType.Income);
		}

		/// <exception cref="InvalidOperationException">When the account type or currency is invalid.</exception>
		private async Task ValidateAccountTypeAndCurrencyAsync(CreateEditAccountInputDTO model)
		{
			bool isAccountTypeValid = await this.accountTypesRepo.All()
				.AnyAsync(at => at.Id == model.AccountTypeId && at.OwnerId == model.OwnerId);

			if (!isAccountTypeValid)
				throw new InvalidOperationException(ExceptionMessages.InvalidAccountType);

			bool isCurrencyValid = await this.currenciesRepo.All()
				.AnyAsync(c => c.Id == model.CurrencyId && c.OwnerId == model.OwnerId);

			if (!isCurrencyValid)
				throw new InvalidOperationException(ExceptionMessages.InvalidCurrency);
		}

		/// <exception cref="InvalidOperationException">When the category is invalid.</exception>
		private async Task ValidateCategoryAsync(Guid categoryId, Guid ownerId)
		{
			bool isCategoryValid = await this.categoriesRepo.All()
				.AnyAsync(c => c.Id == categoryId && c.OwnerId == ownerId);

			if (!isCategoryValid)
				throw new InvalidOperationException(ExceptionMessages.InvalidCategory);
		}
	}
}