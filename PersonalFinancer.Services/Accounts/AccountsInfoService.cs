namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using System;
	using System.Threading.Tasks;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

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
		/// Throws Invalid Operation Exception when the account does not exist
		/// or the user is not owner or administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<AccountDetailsLongDTO> GetAccountDetailsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.ProjectTo<AccountDetailsLongDTO>(this.mapper.ConfigurationProvider, new { startDate, endDate })
				.FirstAsync();
		}

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist 
		/// or the user is not owner or administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CreateEditAccountDTO> GetAccountFormDataAsync(
			Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.ProjectTo<CreateEditAccountDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist
		/// or the user is not owner or administrator.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.Select(a => a.Name)
				.FirstAsync();
		}

		/// <summary>
		/// Throws Invalid Operation Exception if the account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> GetAccountOwnerIdAsync(Guid accountId)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted)
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
					.Skip(AccountsPerPage * (page - 1))
					.Take(AccountsPerPage)
					.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalAccountsCount = await this.accountsRepo.All()
					.CountAsync(a => !a.IsDeleted)
			};
		}

		public async Task<int> GetAccountsCountAsync()
			=> await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

		/// <summary>
		/// Throws Invalid Operation Exception when the account does not exist.
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
		/// Throws Invalid Operation Exception when the account does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TransactionsDTO> GetAccountTransactionsAsync(AccountTransactionsFilterDTO dto)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == dto.AccountId && !a.IsDeleted)
				.ProjectTo<TransactionsDTO>(
					this.mapper.ConfigurationProvider, 
					new { startDate = dto.StartDate, endDate = dto.EndDate, page = dto.Page })
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
		/// Throws Invalid Operation Exception when the transaction does not exist
		/// and Argument Exception when the user is not owner or administrator.
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
				throw new ArgumentException(ExceptionMessages.UnauthorizedUser);

			return transaction;
		}

		/// <summary>
		/// Throws Invalid Operation Exception when the user is not owner, 
		/// transaction does not exist or is initial.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CreateEditTransactionDTO> GetTransactionFormDataAsync(Guid transactionId, Guid userId, bool isUserAdmin)
		{
			return await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId && 
							(isUserAdmin || t.OwnerId == userId) && 
							!t.IsInitialBalance)
				.ProjectTo<CreateEditTransactionDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws Invalid Operation Exception if the transaction does not exist.
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
