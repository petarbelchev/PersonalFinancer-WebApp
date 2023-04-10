namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;

	using Data;
	using Data.Enums;
	using Data.Models;
	using static Data.Constants;
	
	using Services.Accounts.Models;
	using Services.Categories.Models;
	using Services.Currencies.Models;
	using Services.Shared.Models;

	public class AccountsService : IAccountsService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public AccountsService(
			PersonalFinancerDbContext context,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.data = context;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		private void ChangeBalance(Account account, decimal amount, TransactionType transactionType)
		{
			if (transactionType == TransactionType.Income)
				account.Balance += amount;
			else if (transactionType == TransactionType.Expense)
				account.Balance -= amount;
		}

		/// <summary>
		/// Throws ArgumentException when User already have Account with given name.
		/// </summary>
		/// <returns>New Account Id.</returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<string> CreateAccount(CreateAccountFormDTO model)
		{
			if (await IsNameExists(model.Name, model.OwnerId))
				throw new ArgumentException(
					$"The User already have Account with {model.Name} name.");

			Account newAccount = new Account()
			{
				Id = Guid.NewGuid().ToString(),
				Name = model.Name.Trim(),
				Balance = model.Balance,
				AccountTypeId = model.AccountTypeId,
				CurrencyId = model.CurrencyId,
				OwnerId = model.OwnerId
			};

			if (newAccount.Balance != 0)
			{
				newAccount.Transactions.Add(new Transaction()
				{
					Id = Guid.NewGuid().ToString(),
					Amount = newAccount.Balance,
					OwnerId = newAccount.OwnerId,
					CategoryId = CategoryConstants.InitialBalanceCategoryId,
					TransactionType = TransactionType.Income,
					CreatedOn = DateTime.UtcNow,
					Refference = CategoryConstants.CategoryInitialBalanceName,
					IsInitialBalance = true
				});
			}

			await data.Accounts.AddAsync(newAccount);
			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CacheKeyValue + model.OwnerId);

			return newAccount.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> CreateTransaction(CreateTransactionInputDTO inputDTO)
		{
			Account? account = await data.Accounts.FindAsync(inputDTO.AccountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			Transaction newTransaction = new Transaction()
			{
				Id = Guid.NewGuid().ToString(),
				Amount = inputDTO.Amount,
				OwnerId = inputDTO.OwnerId,
				CategoryId = inputDTO.CategoryId,
				TransactionType = inputDTO.TransactionType,
				CreatedOn = inputDTO.CreatedOn,
				Refference = inputDTO.Refference.Trim(),
				IsInitialBalance = inputDTO.IsInitialBalance
			};

			account.Transactions.Add(newTransaction);

			if (inputDTO.TransactionType == TransactionType.Income)
				account.Balance += newTransaction.Amount;
			else if (inputDTO.TransactionType == TransactionType.Expense)
				account.Balance -= newTransaction.Amount;

			await data.SaveChangesAsync();

			return newTransaction.Id;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteAccount(string accountId, bool shouldDeleteTransactions = false, string? userId = null)
		{
			Account account = await data.Accounts
				.FirstAsync(a => a.Id == accountId && !a.IsDeleted);

			if (userId != null && account.OwnerId != userId)
				throw new ArgumentException("Can't delete someone else account.");

			if (shouldDeleteTransactions)
				data.Accounts.Remove(account);
			else
				account.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(AccountConstants.CacheKeyValue + userId);
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when Owner Id is passed and User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<decimal> DeleteTransaction(string transactionId, string? ownerId = null)
		{
			Transaction transaction = await data.Transactions
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == transactionId);

			if (ownerId != null && transaction.OwnerId != ownerId)
				throw new ArgumentException("User is now transaction's owner");

			data.Transactions.Remove(transaction);

			if (transaction.TransactionType == TransactionType.Income)
				transaction.TransactionType = TransactionType.Expense;
			else
				transaction.TransactionType = TransactionType.Income;

			ChangeBalance(transaction.Account, transaction.Amount, transaction.TransactionType);

			await data.SaveChangesAsync();

			return transaction.Account.Balance;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does now exist,
		/// and ArgumentException when User already have Account with given name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditAccount(EditAccountFormDTO model)
		{
			Account account = await data.Accounts.FirstAsync(a => a.Id == model.Id);

			if (account.Name != model.Name && await IsNameExists(model.Name, model.OwnerId))
				throw new ArgumentException($"The User already have Account with {model.Name} name.");

			account.Name = model.Name.Trim();
			account.CurrencyId = model.CurrencyId;
			account.AccountTypeId = model.AccountTypeId;

			if (account.Balance != model.Balance)
			{
				decimal amountOfChange = model.Balance - account.Balance;
				account.Balance = model.Balance;

				Transaction? transaction = await data.Transactions
					.FirstOrDefaultAsync(t => t.AccountId == account.Id && t.IsInitialBalance);

				if (transaction == null)
				{
					var initialBalance = new Transaction
					{
						Id = Guid.NewGuid().ToString(),
						OwnerId = account.OwnerId,
						Amount = amountOfChange,
						CategoryId = CategoryConstants.InitialBalanceCategoryId,
						CreatedOn = DateTime.UtcNow,
						Refference = CategoryConstants.CategoryInitialBalanceName,
						TransactionType = amountOfChange < 0 ? TransactionType.Expense : TransactionType.Income,
						IsInitialBalance = true
					};

					account.Transactions.Add(initialBalance);
				}
				else
				{
					transaction.Amount += amountOfChange;
				}
			}

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction or Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task EditTransaction(EditTransactionInputDTO inputDTO)
		{
			Transaction transactionInDb = await data.Transactions
				.Include(t => t.Account)
				.FirstAsync(t => t.Id == inputDTO.Id);

			if (inputDTO.AccountId != transactionInDb.AccountId
				|| inputDTO.TransactionType != transactionInDb.TransactionType
				|| inputDTO.Amount != transactionInDb.Amount)
			{
				TransactionType opositeTransactionType = TransactionType.Income;

				if (transactionInDb.TransactionType == TransactionType.Income)
					opositeTransactionType = TransactionType.Expense;

				ChangeBalance(transactionInDb.Account, transactionInDb.Amount, opositeTransactionType);

				if (inputDTO.AccountId != transactionInDb.AccountId)
				{
					Account newAccount = await data.Accounts.FirstAsync(a => a.Id == inputDTO.AccountId);
					transactionInDb.Account = newAccount;
				}

				ChangeBalance(transactionInDb.Account, inputDTO.Amount, inputDTO.TransactionType);
			}

			transactionInDb.Refference = inputDTO.Refference.Trim();
			transactionInDb.AccountId = inputDTO.AccountId;
			transactionInDb.CategoryId = inputDTO.CategoryId;
			transactionInDb.Amount = inputDTO.Amount;
			transactionInDb.CreatedOn = inputDTO.CreatedOn;
			transactionInDb.TransactionType = inputDTO.TransactionType;

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDetailsOutputDTO> GetAccountDetails(
			AccountDetailsInputDTO inputDTO, string? ownerId = null)
		{
			bool isUserAdmin = false;

			if (ownerId == null)
				isUserAdmin = true;

			return await data.Accounts
				.Where(a => a.Id == inputDTO.Id
							&& !a.IsDeleted
							&& (isUserAdmin || a.OwnerId == ownerId))
				.Select(a => new AccountDetailsOutputDTO
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name,
					StartDate = inputDTO.StartDate,
					EndDate = inputDTO.EndDate,
					AllTransactionsCount = a.Transactions
						.Count(t => t.CreatedOn >= inputDTO.StartDate && t.CreatedOn <= inputDTO.EndDate),
					OwnerId = a.OwnerId,
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= inputDTO.StartDate && t.CreatedOn <= inputDTO.EndDate)
						.OrderByDescending(t => t.CreatedOn)
						.Take(10)
						.Select(t => new TransactionTableDTO
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn,
							CategoryName = t.Category.Name + (t.Category.IsDeleted ?
								" (Deleted)"
								: string.Empty),
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference
						})
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDTO> GetAccount(string accountId)
		{
			return await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<AccountDTO>(a))
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist 
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<EditAccountFormDTO> GetAccountForm(string accountId, string? ownerId = null)
		{
			bool isUserAdmin = false;

			if (ownerId == null)
				isUserAdmin = true;

			return await data.Accounts
				.Where(a => a.Id == accountId
							&& (isUserAdmin || a.OwnerId == ownerId))
				.Select(a => new EditAccountFormDTO
				{
					Name = a.Name,
					OwnerId = a.OwnerId,
					CurrencyId = a.CurrencyId,
					AccountTypeId = a.AccountTypeId,
					Balance = a.Balance,
					Currencies = a.Owner.Currencies
						.Where(c => !c.IsDeleted)
						.Select(c => new CurrencyOutputDTO
						{
							Id = c.Id,
							Name = c.Name
						}),
					AccountTypes = a.Owner.AccountTypes
						.Where(at => !at.IsDeleted)
						.Select(at => new AccountTypeOutputDTO
						{
							Id = at.Id,
							Name = at.Name
						})
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist or Start date is after End date.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountTransactionsOutputDTO> GetAccountTransactions(AccountTransactionsInputDTO inputDTO)
		{
			if (inputDTO.StartDate > inputDTO.EndDate)
				throw new InvalidOperationException("Start date is after End date.");

			AccountTransactionsOutputDTO outputDTO = await data.Accounts
				.Where(a => a.Id == inputDTO.Id && !a.IsDeleted)
				.Select(a => new AccountTransactionsOutputDTO
				{
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= inputDTO.StartDate && t.CreatedOn <= inputDTO.EndDate)
						.OrderByDescending(t => t.CreatedOn)
						.Skip(inputDTO.ElementsPerPage * (inputDTO.Page - 1))
						.Take(inputDTO.ElementsPerPage)
						.Select(t => new TransactionTableDTO
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn,
							CategoryName = t.Category.Name + (t.Category.IsDeleted ?
								" (Deleted)"
								: string.Empty),
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference
						}),
					Page = inputDTO.Page,
					AllTransactionsCount = a.Transactions
						.Count(t => t.CreatedOn >= inputDTO.StartDate && t.CreatedOn <= inputDTO.EndDate),
				})
				.FirstAsync();

			return outputDTO;
		}

		public async Task<AccountCardsOutputDTO> GetUsersAccountCards(int page, int elementsPerPage)
		{
			return new AccountCardsOutputDTO
			{
				Accounts = await data.Accounts
					.Where(a => !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Skip(elementsPerPage * (page - 1))
					.Take(elementsPerPage)
					.Select(a => mapper.Map<AccountCardExtendedDTO>(a))
					.ToArrayAsync(),
				Page = page,
				AllAccountsCount = data.Accounts.Count(a => !a.IsDeleted)
			};
		}

		public async Task<IEnumerable<CurrencyCashFlowDTO>> GetUsersCurrenciesCashFlow()
		{
			return await data.Transactions
				.GroupBy(t => t.Account.Currency.Name)
				.Select(t => new CurrencyCashFlowDTO
				{
					Name = t.Key,
					Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
					Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
				})
				.OrderBy(c => c.Name)
				.ToArrayAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when User does not exist or Start date is after End date.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserTransactionsApiOutputDTO> GetUserTransactionsApi(UserTransactionsApiInputDTO inputDTO)
		{
			if (inputDTO.StartDate > inputDTO.EndDate)
				throw new InvalidOperationException("Start date is after End date");

			return await data.Users
				.Where(u => u.Id == inputDTO.Id)
				.Select(u => new UserTransactionsApiOutputDTO
				{
					Transactions = u.Transactions
						.Where(t => t.CreatedOn >= inputDTO.StartDate && t.CreatedOn <= inputDTO.EndDate)
						.OrderByDescending(t => t.CreatedOn)
						.Skip(inputDTO.ElementsPerPage * (inputDTO.Page - 1))
						.Take(inputDTO.ElementsPerPage)
						.Select(t => new TransactionTableDTO
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = t.Account.Currency.Name,
							CategoryName = t.Category.Name + (t.Category.IsDeleted ?
								" (Deleted)"
								: string.Empty),
							CreatedOn = t.CreatedOn,
							Refference = t.Refference,
							TransactionType = t.TransactionType.ToString()
						}),
					Page = inputDTO.Page,
					AllTransactionsCount = u.Transactions
						.Count(t => t.CreatedOn >= inputDTO.StartDate && t.CreatedOn <= inputDTO.EndDate),
				})
				.FirstAsync();
		}

		public async Task<UserTransactionsOutputDTO> GetUserTransactions(UserTransactionsInputDTO input)
		{
			return await data.Users
				.Where(u => u.Id == input.Id)
				.Select(u => new UserTransactionsOutputDTO
				{
					Id = u.Id,
					StartDate = input.StartDate,
					EndDate = input.EndDate,
					AllTransactionsCount = u.Transactions
						.Count(t => t.CreatedOn >= input.StartDate && t.CreatedOn <= input.EndDate),
					Transactions = u.Transactions
						.Where(t => t.CreatedOn >= input.StartDate && t.CreatedOn <= input.EndDate)
						.OrderByDescending(t => t.CreatedOn)
						.Take(input.ElementsPerPage)
						.Select(t => new TransactionTableDTO
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = t.Account.Currency.Name,
							CategoryName = t.Category.Name,
							CreatedOn = t.CreatedOn,
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference
						})
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<DeleteAccountDTO> GetDeleteAccountData(string accountId, string? ownerId = null)
		{
			bool isUserAdmin = false;

			if (ownerId == null)
				isUserAdmin = true;

			return await data.Accounts
				.Where(a => a.Id == accountId
							&& (isUserAdmin || a.OwnerId == ownerId))
				.Select(a => mapper.Map<DeleteAccountDTO>(a))
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CreateAccountFormDTO> GetEmptyAccountForm(string userId)
		{
			return await data.Users.Where(u => u.Id == userId)
				.Select(u => new EditAccountFormDTO
				{
					OwnerId = u.Id,
					AccountTypes = u.AccountTypes
						.Where(at => !at.IsDeleted)
						.Select(at => new AccountTypeOutputDTO
						{
							Id = at.Id,
							Name = at.Name
						}),
					Currencies = u.Currencies
						.Where(c => !c.IsDeleted)
						.Select(c => new CurrencyOutputDTO
						{
							Id = c.Id,
							Name = c.Name
						})
				})
				.FirstAsync();
		}

		public async Task<EmptyTransactionFormDTO> GetEmptyTransactionForm(string userId)
		{
			return await data.Users.Where(u => u.Id == userId)
				.Select(u => new EmptyTransactionFormDTO
				{
					OwnerId = u.Id,
					CreatedOn = DateTime.UtcNow,
					UserAccounts = u.Accounts
						.Where(a => !a.IsDeleted)
						.Select(a => mapper.Map<AccountDTO>(a)),
					UserCategories = u.Categories
						.Where(c => !c.IsDeleted)
						.Select(c => mapper.Map<CategoryOutputDTO>(c))
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<FulfilledTransactionFormDTO> GetFulfilledTransactionForm(string transactionId)
		{
			return await data.Transactions.Where(t => t.Id == transactionId)
				.Select(t => new FulfilledTransactionFormDTO
				{
					OwnerId = t.OwnerId,
					AccountId = t.AccountId,
					CategoryId = t.CategoryId,
					Amount = t.Amount,
					CreatedOn = t.CreatedOn,
					TransactionType = t.TransactionType,
					Refference = t.Refference,
					UserAccounts = t.Account.IsDeleted ?
						new AccountDTO[] { new AccountDTO { Id = t.AccountId, Name = t.Account.Name } }
						: t.Owner.Accounts.Where(a => !a.IsDeleted).Select(a => mapper.Map<AccountDTO>(a)),
					UserCategories = t.IsInitialBalance ?
						new CategoryOutputDTO[] { new CategoryOutputDTO { Id = t.CategoryId, Name = t.Category.Name } }
						: t.Owner.Categories.Where(c => !c.IsDeleted).Select(c => mapper.Map<CategoryOutputDTO>(c))
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> GetOwnerId(string accountId)
		{
			Account account = await data.Accounts.FirstAsync(a => a.Id == accountId);

			return account.OwnerId;
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionDetailsDTO> GetTransactionDetails(string transactionId)
		{
			return await data.Transactions
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionDetailsDTO>(mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<bool> IsAccountDeleted(string accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			return account.IsDeleted;
		}

		/// <summary>
		/// Throws ArgumentNullException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<bool> IsAccountOwner(string userId, string accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
				throw new InvalidOperationException("Account does not exist.");

			return account.OwnerId == userId;
		}

		private async Task<bool> IsNameExists(string name, string userId)
		{
			var names = await data.Accounts
				.Where(a => a.OwnerId == userId)
				.Select(a => a.Name.ToLower())
				.ToArrayAsync();

			return names.Contains(name.ToLower().Trim());
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDetailsOutputDTO> GetAccountDetailsForReturn(AccountDetailsInputDTO inputDTO)
		{
			return await data.Accounts
				.Where(a => a.Id == inputDTO.Id)
				.Select(a => new AccountDetailsOutputDTO
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name,
					StartDate = inputDTO.StartDate,
					EndDate = inputDTO.EndDate
				})
				.FirstAsync();
		}
	}
}