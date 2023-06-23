namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using System;
	using System.Threading.Tasks;
	using static PersonalFinancer.Services.Constants;

	public class AccountsInfoService : IAccountsInfoService
	{
		private readonly IEfRepository<Transaction> transactionsRepo;
		private readonly IEfRepository<Account> accountsRepo;
		private readonly IMapper mapper;

		public AccountsInfoService(
			IEfRepository<Transaction> transactionsRepo,
			IEfRepository<Account> accountsRepo,
			IMapper mapper)
		{
			this.transactionsRepo = transactionsRepo;
			this.accountsRepo = accountsRepo;
			this.mapper = mapper;
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDetailsServiceModel> GetAccountDetailsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin)
		{
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();

			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.Select(a => new AccountDetailsServiceModel
				{
					Id = a.Id,
					Name = a.Name,
					OwnerId = a.OwnerId,
					Balance = a.Balance,
					CurrencyName = a.Currency.Name,
					AccountTypeName = a.AccountType.Name,
					StartDate = startDate,
					EndDate = endDate,
					TotalAccountTransactions = a.Transactions
						.Count(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc),
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
						.OrderByDescending(t => t.CreatedOn)
						.Take(PaginationConstants.TransactionsPerPage)
						.Select(t => new TransactionTableServiceModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = a.Currency.Name,
							CreatedOn = t.CreatedOn.ToLocalTime(),
							CategoryName = t.Category.Name + (t.Category.IsDeleted ?
								" (Deleted)"
								: string.Empty),
							TransactionType = t.TransactionType.ToString(),
							Reference = t.Reference
						})
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist 
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountFormShortServiceModel> GetAccountFormDataAsync(
			Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && (isUserAdmin || a.OwnerId == userId))
				.ProjectTo<AccountFormShortServiceModel>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist
		/// or User is not owner or Administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && (isUserAdmin || a.OwnerId == userId))
				.Select(a => a.Name)
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> GetAccountOwnerIdAsync(Guid accountId)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId)
				.Select(a => a.OwnerId)
				.FirstAsync();
		}

		public async Task<UsersAccountsCardsServiceModel> GetAccountsCardsDataAsync(int page)
		{
			return new UsersAccountsCardsServiceModel
			{
				Accounts = await this.accountsRepo.All()
					.Where(a => !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Skip(PaginationConstants.AccountsPerPage * (page - 1))
					.Take(PaginationConstants.AccountsPerPage)
					.ProjectTo<AccountCardServiceModel>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalUsersAccountsCount = await this.accountsRepo.All()
					.CountAsync(a => !a.IsDeleted)
			};
		}

		public async Task<int> GetAccountsCountAsync()
			=> await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDetailsShortServiceModel> GetAccountShortDetailsAsync(Guid accountId)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId)
				.ProjectTo<AccountDetailsShortServiceModel>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<TransactionsServiceModel> GetAccountTransactionsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, int page = 1)
		{
			IQueryable<Transaction> query = this.transactionsRepo.All()
				.Where(t => t.AccountId == accountId && !t.Account.IsDeleted);

			return await this.GetTransactions(query, startDate, endDate, page);
		}

		public async Task<IEnumerable<CurrencyCashFlowServiceModel>> GetCashFlowByCurrenciesAsync()
		{
			return await this.transactionsRepo.All()
				.GroupBy(t => t.Account.Currency.Name)
				.Select(t => new CurrencyCashFlowServiceModel
				{
					Name = t.Key,
					Incomes = t
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount),
					Expenses = t
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount)
				})
				.OrderBy(c => c.Name)
				.ToArrayAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Transaction does not exist
		/// and ArgumentException when the User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionDetailsServiceModel> GetTransactionDetailsAsync(
			Guid transactionId, Guid ownerId, bool isUserAdmin)
		{
			TransactionDetailsServiceModel? transaction = await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionDetailsServiceModel>(this.mapper.ConfigurationProvider)
				.FirstOrDefaultAsync() ??
					throw new InvalidOperationException("Transaction does not exist.");

			if (!isUserAdmin && transaction.OwnerId != ownerId)
				throw new ArgumentException("User is not transaction's owner.");

			transaction.CreatedOn = transaction.CreatedOn.ToLocalTime();

			return transaction;
		}

		/// <summary>
		/// Throws InvalidOperationException when the user is not owner, transaction does not exist or is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionFormModel> GetTransactionFormDataAsync(Guid transactionId, Guid ownerId)
		{
			return await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId && t.OwnerId == ownerId && !t.IsInitialBalance)
				.Select(t => new TransactionFormModel
				{
					OwnerId = t.OwnerId,
					AccountId = t.AccountId,
					CategoryId = t.CategoryId,
					Amount = t.Amount,
					CreatedOn = t.CreatedOn.ToLocalTime(),
					TransactionType = t.TransactionType,
					Reference = t.Reference,
					IsInitialBalance = t.IsInitialBalance,
					UserAccounts = t.Owner.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)
						.Select(a => this.mapper.Map<AccountServiceModel>(a)),
					UserCategories = t.Owner.Categories
						.Where(c => !c.IsDeleted)
						.OrderBy(c => c.Name)
						.Select(c => this.mapper.Map<CategoryServiceModel>(c))
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if Transaction does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> GetTransactionOwnerIdAsync(Guid transactionId)
		{
			// TODO: Write Unit tests!

			return await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId)
				.Select(t => t.OwnerId)
				.FirstAsync();
		}

		public async Task<IEnumerable<AccountCardServiceModel>> GetUserAccountsAsync(Guid userId)
		{
			return await this.accountsRepo.All()
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => this.mapper.Map<AccountCardServiceModel>(a))
				.ToArrayAsync();
		}

		public async Task<TransactionsServiceModel> GetUserTransactionsAsync(
			Guid userId, UserTransactionsInputModel inputModel, int page = 1)
		{
			IQueryable<Transaction> query = this.transactionsRepo.All()
				.Where(t => t.OwnerId == userId);

			if (!string.IsNullOrWhiteSpace(inputModel.AccountId))
				query = query.Where(t => t.AccountId == Guid.Parse(inputModel.AccountId!));

			if (!string.IsNullOrWhiteSpace(inputModel.CurrencyId))
				query = query.Where(t => t.Account.CurrencyId == Guid.Parse(inputModel.CurrencyId!));

			if (!string.IsNullOrWhiteSpace(inputModel.CategoryId))
				query = query.Where(t => t.CategoryId == Guid.Parse(inputModel.CategoryId!));

			if (!string.IsNullOrWhiteSpace(inputModel.AccountTypeId))
				query = query.Where(t => t.Account.AccountTypeId == Guid.Parse(inputModel.AccountTypeId!));

			return await this.GetTransactions(
				query,
				inputModel.StartDate ?? throw new InvalidOperationException("Start Date cannot be null."),
				inputModel.EndDate ?? throw new InvalidOperationException("End Date cannot be null"), 
				page);
		}

		private async Task<TransactionsServiceModel> GetTransactions(
			 IQueryable<Transaction> query, DateTime startDate, DateTime endDate, int page = 1)
		{
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();

			query = query.Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc);

			return new TransactionsServiceModel()
			{
				Transactions = await query
					.OrderByDescending(t => t.CreatedOn)
					.Skip(PaginationConstants.TransactionsPerPage * (page - 1))
					.Take(PaginationConstants.TransactionsPerPage)
					.Select(t => new TransactionTableServiceModel
					{
						Id = t.Id,
						Amount = t.Amount,
						AccountCurrencyName = t.Account.Currency.Name,
						CreatedOn = t.CreatedOn.ToLocalTime(),
						CategoryName = t.Category.Name + (t.Category.IsDeleted ?
							" (Deleted)"
							: string.Empty),
						TransactionType = t.TransactionType.ToString(),
						Reference = t.Reference
					})
					.ToArrayAsync(),
				TotalTransactionsCount = await query.CountAsync()
			};
		}
	}
}
