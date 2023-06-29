﻿namespace PersonalFinancer.Services.Accounts
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using PersonalFinancer.Common.Messages;
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
		/// Throws Argument Exception when the user already have account with the given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<Guid> CreateAccountAsync(CreateEditAccountDTO model)
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

			return newAccount.Id;
		}

		/// <summary>
		/// Throws Invalid Operation Exception if the account does not exist.
		/// </summary>
		/// <returns>New transaction Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> CreateTransactionAsync(CreateEditTransactionDTO model)
		{
			Account account = await this.FindAccountAsync(model.AccountId);

			await this.ValidateCategoryAsync(model.CategoryId, model.OwnerId);

			Transaction newTransaction = this.mapper.Map<Transaction>(model);

			await this.transactionsRepo.AddAsync(newTransaction);

			ChangeAccountBalance(account, newTransaction.Amount, model.TransactionType);

			await this.accountsRepo.SaveChangesAsync();

			return newTransaction.Id;
		}

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist
		/// and Argument Exception when the user is not owner or administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccountAsync(
			Guid accountId, Guid userId, bool isUserAdmin, bool shouldDeleteTransactions = false)
		{
			Account account = await this.FindAccountAsync(accountId);

			if (!isUserAdmin && account.OwnerId != userId)
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			if (shouldDeleteTransactions)
				this.accountsRepo.Remove(account);
			else
				account.IsDeleted = true;

			await this.accountsRepo.SaveChangesAsync();
		}

		/// <summary>
		/// Throws Invalid Operation Exception when the transaction does not exist
		/// and Argument Exception when the user is not owner or administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<decimal> DeleteTransactionAsync(Guid transactionId, Guid userId, bool isUserAdmin)
		{
			Transaction transaction = await this.transactionsRepo.All()
			   .Include(t => t.Account)
			   .FirstAsync(t => t.Id == transactionId);

			if (!isUserAdmin && transaction.OwnerId != userId)
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			this.transactionsRepo.Remove(transaction);

			RestoreAccountBalance(transaction);

			await this.transactionsRepo.SaveChangesAsync();

			return transaction.Account.Balance;
		}

		/// <summary>
		/// Throws Invalid Operation Exception when the account does now exist,
		/// and Argument Exception when the user already have account with the given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditAccountAsync(Guid accountId, CreateEditAccountDTO model)
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

		/// <summary>
		/// Throws Invalid Operation Exception when the transaction, category or account does not exist
		/// or the transaction is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditTransactionAsync(Guid transactionId, CreateEditTransactionDTO model)
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
			transactionInDb.CreatedOn = model.CreatedOn.ToUniversalTime();

			await this.transactionsRepo.SaveChangesAsync();
		}

		private static Transaction CreateInitialTransaction(Guid accountId, Guid ownerId, decimal amount)
		{
			return new Transaction()
			{
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
		/// Throws Invalid Operation Exception if the account does not exist.
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
		/// Throws Invalid Operation Exception if the account type or currency is invalid.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ValidateAccountTypeAndCurrencyAsync(CreateEditAccountDTO model)
		{
			bool isAccountTypeValid = await this.accountTypesRepo.All().AnyAsync(at =>
				at.Id == model.AccountTypeId
				&& at.OwnerId == model.OwnerId
				&& at.IsDeleted == false);

			if (!isAccountTypeValid)
				throw new InvalidOperationException(ExceptionMessages.InvalidAccountType);

			bool isCurrencyValid = await this.currenciesRepo.All().AnyAsync(c =>
				c.Id == model.CurrencyId
				&& c.OwnerId == model.OwnerId
				&& c.IsDeleted == false);

			if (!isCurrencyValid)
				throw new InvalidOperationException(ExceptionMessages.InvalidCurrency);
		}

		/// <summary>
		/// Throws Invalid Operation Exception if the category is invalid.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ValidateCategoryAsync(Guid categoryId, Guid ownerId)
		{
			bool isCategoryValid = await this.categoriesRepo.All().AnyAsync(c =>
				c.Id == categoryId
				&& c.OwnerId == ownerId
				&& c.IsDeleted == false);

			if (!isCategoryValid)
				throw new InvalidOperationException(ExceptionMessages.InvalidCategory);
		}
	}
}