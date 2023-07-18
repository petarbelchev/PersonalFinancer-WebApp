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

		public async Task<AccountDetailsDTO> GetAccountDetailsAsync(
			Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.ProjectTo<AccountDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<CreateEditAccountOutputDTO> GetAccountFormDataAsync(
			Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.ProjectTo<CreateEditAccountOutputDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted && (isUserAdmin || a.OwnerId == userId))
				.Select(a => a.Name)
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

		public async Task<TransactionsDTO> GetAccountTransactionsAsync(AccountTransactionsFilterDTO dto)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == dto.AccountId && !a.IsDeleted)
				.ProjectTo<TransactionsDTO>(
					this.mapper.ConfigurationProvider, 
					new { fromLocalTime = dto.FromLocalTime, toLocalTime = dto.ToLocalTime, page = dto.Page })
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

		public async Task<CreateEditTransactionOutputDTO> GetTransactionFormDataAsync(Guid transactionId, Guid userId, bool isUserAdmin)
		{
			return await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId && 
							(isUserAdmin || t.OwnerId == userId) && 
							!t.IsInitialBalance)
				.ProjectTo<CreateEditTransactionOutputDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<Guid> GetTransactionOwnerIdAsync(Guid transactionId)
		{
			return await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId)
				.Select(t => t.OwnerId)
				.FirstAsync();
		}
	}
}
