﻿namespace PersonalFinancer.Services.User
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
	using static PersonalFinancer.Data.Constants.CategoryConstants;
	using static PersonalFinancer.Services.Constants;

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

		public async Task<UsersInfoDTO> GetUsersInfoAsync(int page)
		{
			return new UsersInfoDTO
			{
				Users = await this.usersRepo.All()
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Skip(PaginationConstants.UsersPerPage * (page - 1))
					.Take(PaginationConstants.UsersPerPage)
					.ProjectTo<UserInfoDTO>(this.mapper.ConfigurationProvider)
					.ToListAsync(),
				TotalUsersCount = await this.usersRepo.All().CountAsync()
			};
		}

		public async Task<TransactionsDTO> GetUserTransactionsAsync(TransactionsFilterDTO dto)
		{
			IQueryable<Transaction> query = this.transactionsRepo.All().Where(t =>
				t.OwnerId == dto.UserId &&
				t.CreatedOn >= dto.StartDate.ToUniversalTime() &&
				t.CreatedOn <= dto.EndDate.ToUniversalTime() &&
				(dto.AccountId == null || t.AccountId == dto.AccountId) &&
				(dto.CurrencyId == null || t.Account.CurrencyId == dto.CurrencyId) &&
				(dto.CategoryId == null || t.CategoryId == dto.CategoryId) &&
				(dto.AccountTypeId == null || t.Account.AccountTypeId == dto.AccountTypeId));

			var result = new TransactionsDTO
			{
				Transactions = await query
					.OrderByDescending(t => t.CreatedOn)
					.Skip(PaginationConstants.TransactionsPerPage * (dto.Page - 1))
					.Take(PaginationConstants.TransactionsPerPage)
					.ProjectTo<TransactionTableDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync(),
				TotalTransactionsCount = await query.CountAsync()
			};

			return result;
		}

		public async Task<TransactionsPageDTO> GetUserTransactionsPageDataAsync(TransactionsFilterDTO dto)
		{
			TransactionsPageDTO resultDTO = await this.usersRepo.All()
				.Where(u => u.Id == dto.UserId)
				.Select(u => new TransactionsPageDTO
				{
					OwnerAccounts = u.Accounts
						.Where(a => !a.IsDeleted || a.Transactions.Any())
						.Select(a => this.mapper.Map<AccountDropdownDTO>(a)),
					OwnerAccountTypes = u.AccountTypes
						.Where(at => !at.IsDeleted || at.Accounts.Any(a => a.Transactions.Any()))
						.Select(at => this.mapper.Map<AccountTypeDropdownDTO>(at)),
					OwnerCurrencies = u.Currencies
						.Where(c => !c.IsDeleted || c.Accounts.Any(a => a.Transactions.Any()))
						.Select(c => this.mapper.Map<CurrencyDropdownDTO>(c)),
					OwnerCategories = u.Categories
						.Where(c => !c.IsDeleted || c.Transactions.Any())
						.Select(c => this.mapper.Map<CategoryDropdownDTO>(c))
						.ToList()
				})
				.FirstAsync();

			resultDTO.OwnerCategories.Add(await this.categoriesRepo.All()
				.Where(c => c.Id == Guid.Parse(InitialBalanceCategoryId))
				.ProjectTo<CategoryDropdownDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync());

			TransactionsDTO transactionsDTO = await this.GetUserTransactionsAsync(dto);
			this.mapper.Map(transactionsDTO, resultDTO);

			return resultDTO;
		}

		public async Task<AccountsAndCategoriesDropdownDTO> GetUserAccountsAndCategoriesDropdownDataAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<AccountsAndCategoriesDropdownDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<AccountTypesAndCurrenciesDropdownDTO> GetUserAccountTypesAndCurrenciesDropdownDataAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<AccountTypesAndCurrenciesDropdownDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		public async Task<UserDashboardDTO> GetUserDashboardDataAsync(Guid userId, DateTime startDate, DateTime endDate)
		{
			var dto = new UserDashboardDTO
			{
				Accounts = await this.accountsRepo.All()
					.Where(a => a.OwnerId == userId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.ProjectTo<AccountCardDTO>(this.mapper.ConfigurationProvider)
					.ToArrayAsync()
			};

			IQueryable<Transaction> query = this.transactionsRepo.All().Where(t =>
				t.OwnerId == userId
				&& t.CreatedOn >= startDate.ToUniversalTime()
				&& t.CreatedOn <= endDate.ToUniversalTime());

			dto.LastTransactions = await query
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.ProjectTo<TransactionTableDTO>(this.mapper.ConfigurationProvider)
				.ToArrayAsync();

			dto.CurrenciesCashFlow = await query
				.GroupBy(t => t.Account.Currency.Name)
				.Select(group => new CurrencyCashFlowWithExpensesByCategoriesDTO
				{
					Name = group.Key,
					Incomes = group
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount),
					Expenses = group
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount),
					ExpensesByCategories = group
						.Where(t => t.TransactionType == TransactionType.Expense)
						.GroupBy(t => t.Category.Name)
						.Select(categoryGroup => new CategoryExpensesDTO
						{
							CategoryName = categoryGroup.Key,
							ExpensesAmount = categoryGroup.Sum(t => t.Amount)
						})
				})
				.OrderBy(c => c.Name)
				.ToArrayAsync();

			return dto;
		}

		public Task<UserDropdownDTO> GetUserDropdownDataAsync(Guid userId)
		{
			return this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<UserDropdownDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserDetailsDTO> UserDetailsAsync(Guid userId)
		{
			return await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
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