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
		public async Task<AccountDetailsLongDTO> GetAccountDetailsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin)
		{
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();

			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.Select(a => new AccountDetailsLongDTO
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
						.Select(t => new TransactionTableDTO
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
		public async Task<CreateEditAccountDTO> GetAccountFormDataAsync(
			Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && (isUserAdmin || a.OwnerId == userId))
				.ProjectTo<CreateEditAccountDTO>(this.mapper.ConfigurationProvider)
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

		public async Task<AccountsCardsDTO> GetAccountsCardsDataAsync(int page)
		{
			return new AccountsCardsDTO
			{
				Accounts = await this.accountsRepo.All()
					.Where(a => !a.IsDeleted)
					.OrderBy(a => a.Name)
					.Skip(PaginationConstants.AccountsPerPage * (page - 1))
					.Take(PaginationConstants.AccountsPerPage)
					.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalAccountsCount = await this.accountsRepo.All()
					.CountAsync(a => !a.IsDeleted)
			};
		}

		public async Task<int> GetAccountsCountAsync()
			=> await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDetailsShortDTO> GetAccountShortDetailsAsync(Guid accountId)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId)
				.ProjectTo<AccountDetailsShortDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException when Account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionsDTO> GetAccountTransactionsAsync(AccountTransactionsFilterDTO dto)
		{
			DateTime startDateUtc = dto.StartDate.ToUniversalTime();
			DateTime endDateUtc = dto.EndDate.ToUniversalTime();

			return await this.accountsRepo.All()
				.Where(a => a.Id == dto.AccountId && !a.IsDeleted)
				.Select(a => new TransactionsDTO()
				{
					Transactions = a.Transactions
						.Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
						.OrderByDescending(t => t.CreatedOn)
						.Skip(PaginationConstants.TransactionsPerPage * (dto.Page - 1))
						.Take(PaginationConstants.TransactionsPerPage)
						.Select(t => this.mapper.Map<TransactionTableDTO>(t)),
					TotalTransactionsCount = a.Transactions
						.Count(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
				})
				.FirstAsync();
		}

		public async Task<IEnumerable<CurrencyCashFlowDTO>> GetCashFlowByCurrenciesAsync()
		{
			return await this.transactionsRepo.All()
				.GroupBy(t => t.Account.Currency.Name)
				.Select(t => new CurrencyCashFlowDTO
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
		public async Task<TransactionDetailsDTO> GetTransactionDetailsAsync(
			Guid transactionId, Guid ownerId, bool isUserAdmin)
		{
			TransactionDetailsDTO transaction = await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			if (!isUserAdmin && transaction.OwnerId != ownerId)
				throw new ArgumentException("The user is not transaction's owner.");

			transaction.CreatedOn = transaction.CreatedOn.ToLocalTime();

			return transaction;
		}

		/// <summary>
		/// Throws InvalidOperationException when the user is not owner, transaction does not exist or is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CreateEditTransactionDTO> GetTransactionFormDataAsync(Guid transactionId, Guid userId, bool isUserAdmin)
		{
			return await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId && 
							(isUserAdmin || t.OwnerId == userId) && 
							!t.IsInitialBalance)
				.Select(t => new CreateEditTransactionDTO
				{
					OwnerId = t.OwnerId,
					AccountId = t.AccountId,
					CategoryId = t.CategoryId,
					Amount = t.Amount,
					CreatedOn = t.CreatedOn.ToLocalTime(),
					TransactionType = t.TransactionType,
					Reference = t.Reference,
					UserAccounts = t.Owner.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)
						.Select(a => this.mapper.Map<AccountDropdownDTO>(a)),
					UserCategories = t.Owner.Categories
						.Where(c => !c.IsDeleted)
						.OrderBy(c => c.Name)
						.Select(c => this.mapper.Map<CategoryDropdownDTO>(c))
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

		public async Task<IEnumerable<AccountCardDTO>> GetUserAccountsCardsAsync(Guid userId)
		{
			return await this.accountsRepo.All()
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => this.mapper.Map<AccountCardDTO>(a))
				.ToArrayAsync();
		}
	}
}
