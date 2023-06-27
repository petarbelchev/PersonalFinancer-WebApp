namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using Microsoft.EntityFrameworkCore;
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

		public AccountsUpdateService(
			IEfRepository<Account> accountRepository,
			IEfRepository<Transaction> transactionRepository,
			IEfRepository<AccountType> accountTypesRepo,
			IEfRepository<Currency> currenciesRepo,
			IEfRepository<Category> categoriesRepo,
			IMapper mapper)
		{
			this.accountsRepo = accountRepository;
			this.transactionsRepo = transactionRepository;
			this.accountTypesRepo = accountTypesRepo;
			this.currenciesRepo = currenciesRepo;
			this.categoriesRepo = categoriesRepo;
			this.mapper = mapper;
		}

		/// <summary>
		/// Throws ArgumentException when User already have Account with the given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<Guid> CreateAccountAsync(CreateEditAccountDTO model)
		{
			if (await this.IsNameExistAsync(model.Name, model.OwnerId))
				throw new ArgumentException($"The User already have Account with \"{model.Name}\" name.");

			await this.ValidateAccountTypeAndCurrencyAsync(model);

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

			return newAccount.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <returns>New transaction Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> CreateTransactionAsync(CreateEditTransactionDTO model)
		{
			Account account = await this.FindAccountAsync(model.AccountId);

			await this.ValidateCategoryAsync(model.CategoryId, model.OwnerId);

			model.CreatedOn = model.CreatedOn.ToUniversalTime();

			Transaction newTransaction = this.mapper.Map<Transaction>(model);
			newTransaction.Id = Guid.NewGuid();

			await this.transactionsRepo.AddAsync(newTransaction);

			ChangeAccountBalance(account, newTransaction.Amount, model.TransactionType);

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
			Account account = await this.FindAccountAsync(accountId);

			if (!isUserAdmin && account.OwnerId != userId)
				throw new ArgumentException("Can't delete someone else account.");

			if (shouldDeleteTransactions)
				this.accountsRepo.Remove(account);
			else
				account.IsDeleted = true;

			await this.accountsRepo.SaveChangesAsync();
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
				throw new ArgumentException("The user is not transaction's owner");

			this.transactionsRepo.Remove(transaction);

			RestoreAccountBalance(transaction);

			await this.transactionsRepo.SaveChangesAsync();

			return transaction.Account.Balance;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditAccountAsync(Guid accountId, CreateEditAccountDTO model)
		{
			Account account = await this.FindAccountAsync(accountId);

			if (account.Name != model.Name && await this.IsNameExistAsync(model.Name, model.OwnerId))
				throw new ArgumentException($"The User already have Account with \"{model.Name}\" name.");

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

		/// <summary>
		/// Throws InvalidOperationException when Transaction, Category or Account does not exist
		/// or Transaction is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditTransactionAsync(Guid transactionId, CreateEditTransactionDTO model)
		{
			Transaction transactionInDb = await this.transactionsRepo.All()
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == transactionId);

			if (transactionInDb.IsInitialBalance)
				throw new InvalidOperationException("Cannot edit initial balance transaction.");

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
			transactionInDb.CreatedOn = model.CreatedOn.ToUniversalTime();

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

		private static void ChangeAccountBalance(Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task<Account> FindAccountAsync(Guid accountId)
			=> await this.accountsRepo.All().FirstAsync(a => a.Id == accountId && !a.IsDeleted);

		private async Task<bool> IsNameExistAsync(string name, Guid userId)
		{
			return await this.accountsRepo.All()
				.AnyAsync(a => a.OwnerId == userId && a.Name == name);
		}

		private static void RestoreAccountBalance(Transaction transaction)
		{
			ChangeAccountBalance(
				transaction.Account,
				transaction.Amount,
				transaction.TransactionType == TransactionType.Income
					? TransactionType.Expense
					: TransactionType.Income);
		}

		/// <summary>
		/// Throws InvalidOperationException if account type or currency is invalid.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ValidateAccountTypeAndCurrencyAsync(CreateEditAccountDTO model)
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

		/// <summary>
		/// Throws InvalidOperationException if category is invalid.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ValidateCategoryAsync(Guid categoryId, Guid ownerId)
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