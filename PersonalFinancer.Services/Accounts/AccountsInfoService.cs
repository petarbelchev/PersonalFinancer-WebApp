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
			AccountDetailsDTO accountDetails = await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<AccountDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			return isUserAdmin || accountDetails.OwnerId == userId
				? accountDetails
				: throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);
		}

		public async Task<CreateEditAccountOutputDTO> GetAccountFormDataAsync(
			Guid accountId, Guid userId, bool isUserAdmin)
		{
			var accountForm = await this.accountsRepo.All()
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<CreateEditAccountOutputDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			return isUserAdmin || accountForm.OwnerId == userId
				? accountForm
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
			TransactionDetailsDTO transaction = await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId)
				.ProjectTo<TransactionDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			return !isUserAdmin && transaction.OwnerId != ownerId
				? throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser)
				: transaction;
		}

		public async Task<CreateEditTransactionOutputDTO> GetTransactionFormDataAsync(
			Guid transactionId, Guid userId, bool isUserAdmin)
		{
			var transactionDTO = await this.transactionsRepo.All()
				.Where(t => t.Id == transactionId && !t.IsInitialBalance)
				.ProjectTo<CreateEditTransactionOutputDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			return isUserAdmin || transactionDTO.OwnerId == userId
				? transactionDTO
				: throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedUser);
		}
	}
}
