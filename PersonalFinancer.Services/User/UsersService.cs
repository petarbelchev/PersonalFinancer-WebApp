namespace PersonalFinancer.Services.User
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;
	using System.Collections.Generic;
	using static PersonalFinancer.Common.Constants.CategoryConstants;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class UsersService : IUsersService
	{
		private readonly IEfRepository<ApplicationUser> usersRepo;
		private readonly IEfRepository<Account> accountsRepo;
		private readonly IEfRepository<Transaction> transactionsRepo;
		private readonly IEfRepository<Category> categoriesRepo;
		private readonly IMapper mapper;

		public UsersService(
			IEfRepository<ApplicationUser> usersRepo,
			IEfRepository<Account> accountsRepo,
			IEfRepository<Transaction> transactionsRepo,
			IEfRepository<Category> categoriesRepo,
			IMapper mapper)
		{
			this.usersRepo = usersRepo;
			this.accountsRepo = accountsRepo;
			this.transactionsRepo = transactionsRepo;
			this.categoriesRepo = categoriesRepo;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<string>> GetAdminsIdsAsync()
		{
			return await this.usersRepo.All()
				.Where(u => u.IsAdmin)
				.Select(u => u.Id.ToString().ToLower()) // .ToString().ToLower() because of SignalR Hubs
				.ToListAsync();
		}

		public async Task<AccountsAndCategoriesDropdownDTO> GetUserAccountsAndCategoriesDropdownDataAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<AccountsAndCategoriesDropdownDTO>(this.mapper.ConfigurationProvider)
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

		public async Task<AccountTypesAndCurrenciesDropdownDTO> GetUserAccountTypesAndCurrenciesDropdownDataAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<AccountTypesAndCurrenciesDropdownDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<UserDashboardDTO> GetUserDashboardDataAsync(Guid userId, DateTime fromLocalTime, DateTime toLocalTime)
		{
			DateTime fromUtc = fromLocalTime.ToUniversalTime();
			DateTime toUtc = toLocalTime.ToUniversalTime();
		
			var dto = new UserDashboardDTO
			{
				FromLocalTime = fromLocalTime,
				ToLocalTime = toLocalTime,
				Accounts = await this.accountsRepo.All()
					.Where(a => a.OwnerId == userId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				LastTransactions = await this.transactionsRepo.All()
					.Where(t => t.OwnerId == userId
								&& t.CreatedOnUtc >= fromUtc
								&& t.CreatedOnUtc <= toUtc)
					.OrderByDescending(t => t.CreatedOnUtc)
					.Take(5)
					.ProjectTo<TransactionTableDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				CurrenciesCashFlow = await this.accountsRepo.All()
					.Where(a => a.OwnerId == userId 
								&& a.Transactions.Any(t => t.CreatedOnUtc >= fromUtc
														   && t.CreatedOnUtc <= toUtc))
					.GroupBy(a => a.Currency.Name)
					.Select(group => new CurrencyCashFlowWithExpensesByCategoriesDTO
					{
						Name = group.Key,
						Incomes = group
							.SelectMany(a => a.Transactions)
							.Where(t => t.TransactionType == TransactionType.Income
										&& t.CreatedOnUtc >= fromUtc
										&& t.CreatedOnUtc <= toUtc)
							.Sum(t => t.Amount),
						Expenses = group
							.SelectMany(a => a.Transactions)
							.Where(t => t.TransactionType == TransactionType.Expense
										&& t.CreatedOnUtc >= fromUtc
										&& t.CreatedOnUtc <= toUtc)
							.Sum(t => t.Amount),
						ExpensesByCategories = group
							.SelectMany(a => a.Transactions
								.Where(t => t.TransactionType == TransactionType.Expense
											&& t.CreatedOnUtc >= fromUtc
											&& t.CreatedOnUtc <= toUtc))
							.GroupBy(t => t.Category.Name)
							.Select(cGroup => new CategoryExpensesDTO
							{
								CategoryName = cGroup.Key,
								ExpensesAmount = cGroup.Sum(t => t.Amount)
							})
					})
					.OrderBy(c => c.Name)
					.ToArrayAsync()
			};

			return dto;
		}

		public async Task<UserDropdownsDTO> GetUserDropdownsDataAsync(Guid userId)
		{
			UserDropdownsDTO resultDTO = await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<UserDropdownsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			resultDTO.OwnerCategories.Add(await this.categoriesRepo.All()
				.Where(c => c.Id == Guid.Parse(InitialBalanceCategoryId))
				.ProjectTo<CategoryDropdownDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync());

			return resultDTO;
		}

		public async Task<UsersInfoDTO> GetUsersInfoAsync(int page)
		{
			return new UsersInfoDTO
			{
				Users = await this.usersRepo.All()
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Skip(UsersPerPage * (page - 1))
					.Take(UsersPerPage)
					.ProjectTo<UserInfoDTO>(this.mapper.ConfigurationProvider)
					.ToListAsync(),
				TotalUsersCount = await this.usersRepo.All().CountAsync()
			};
		}

		public async Task<TransactionsDTO> GetUserTransactionsAsync(TransactionsFilterDTO dto)
		{
			IQueryable<Transaction> query = this.transactionsRepo.All().Where(t =>
				t.OwnerId == dto.UserId &&
				t.CreatedOnUtc >= dto.FromLocalTime.ToUniversalTime() &&
				t.CreatedOnUtc <= dto.ToLocalTime.ToUniversalTime() &&
				(dto.AccountId == null || t.AccountId == dto.AccountId) &&
				(dto.CurrencyId == null || t.Account.CurrencyId == dto.CurrencyId) &&
				(dto.CategoryId == null || t.CategoryId == dto.CategoryId) &&
				(dto.AccountTypeId == null || t.Account.AccountTypeId == dto.AccountTypeId));

			var result = new TransactionsDTO
			{
				Transactions = await query
					.OrderByDescending(t => t.CreatedOnUtc)
					.Skip(TransactionsPerPage * (dto.Page - 1))
					.Take(TransactionsPerPage)
					.ProjectTo<TransactionTableDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalTransactionsCount = await query.CountAsync()
			};

			return result;
		}

		public async Task<UserDetailsDTO> UserDetailsAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<string> UserFullNameAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.Select(u => $"{u.FirstName} {u.LastName}")
				.FirstAsync();
		}

		public async Task<int> UsersCountAsync()
			=> await this.usersRepo.All().CountAsync();
	}
}