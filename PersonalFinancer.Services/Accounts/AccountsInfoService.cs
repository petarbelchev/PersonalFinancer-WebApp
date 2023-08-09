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
	using System.Linq.Expressions;
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

		public async Task<T> GetAccountDataAsync<T>(Guid accountId, Guid userId, bool isUserAdmin)
			where T : IHaveOwner
		{
			T accountData = await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<T>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			return isUserAdmin || accountData.OwnerId == userId
				? accountData
				: throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);
		}

		public async Task<string> GetAccountNameAsync(Guid accountId, Guid userId, bool isUserAdmin)
		{
			Account account = await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.FirstAsync();

			return isUserAdmin || account.OwnerId == userId 
				? account.Name 
				: throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);
		}

		public async Task<AccountsCardsDTO> GetAccountsCardsDataAsync(int page, string? search)
		{
			var query = this.accountsRepo.All().Where(a => !a.IsDeleted);

			if (search != null)
			{
				search = search.ToLower();

				query = query.Where(a => 
					a.Name.ToLower().Contains(search) ||
					a.Currency.Name.ToLower().Contains(search) ||
					a.AccountType.Name.ToLower().Contains(search));
			}

			return new AccountsCardsDTO
			{
				Accounts = await query
					.OrderBy(a => a.Name)
					.Skip(AccountsPerPage * (page - 1))
					.Take(AccountsPerPage)
					.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalAccountsCount = await query.CountAsync()
			};
		}

		public async Task<int> GetAccountsCountAsync()
			=> await this.accountsRepo.All().CountAsync(a => !a.IsDeleted);

		public async Task<TransactionsDTO> GetAccountTransactionsAsync(AccountTransactionsFilterDTO dto)
		{
			return await this.accountsRepo.All()
				.Where(a => a.Id == dto.Id && !a.IsDeleted)
				.ProjectTo<TransactionsDTO>(
					this.mapper.ConfigurationProvider,
					new
					{
						fromLocalTime = dto.FromLocalTime,
						toLocalTime = dto.ToLocalTime,
						page = dto.Page
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

		public async Task<TransactionDetailsDTO> GetTransactionDetailsAsync(
			Guid transactionId, Guid ownerId, bool isUserAdmin)
		{
			return await this.GetTransactionDataAsync<TransactionDetailsDTO>(
				t => t.Id == transactionId,
				ownerId,
				isUserAdmin);
		}

		public async Task<CreateEditTransactionOutputDTO> GetTransactionFormDataAsync(
			Guid transactionId, Guid userId, bool isUserAdmin)
		{
			return await this.GetTransactionDataAsync<CreateEditTransactionOutputDTO>(
				t => t.Id == transactionId && !t.IsInitialBalance, 
				userId, 
				isUserAdmin);
		}

		/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
		/// <exception cref="InvalidOperationException">When the transaction does not exist.</exception>
		private async Task<T> GetTransactionDataAsync<T>(
			Expression<Func<Transaction, bool>> filterExpression, 
			Guid userId, 
			bool isUserAdmin)
			where T : IHaveOwner
		{
			var transactionDTO = await this.transactionsRepo.All()
				.Where(filterExpression)
				.ProjectTo<T>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			return isUserAdmin || transactionDTO.OwnerId == userId
				? transactionDTO
				: throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);
		}
	}
}
