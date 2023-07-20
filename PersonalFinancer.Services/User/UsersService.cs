﻿namespace PersonalFinancer.Services.User
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using static PersonalFinancer.Common.Constants.CacheConstants;
	using static PersonalFinancer.Common.Constants.CategoryConstants;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class UsersService : IUsersService
	{
		private readonly IEfRepository<ApplicationUser> usersRepo;
		private readonly IEfRepository<Account> accountsRepo;
		private readonly IEfRepository<Transaction> transactionsRepo;
		private readonly IEfRepository<Category> categoriesRepo;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public UsersService(
			IEfRepository<ApplicationUser> usersRepo,
			IEfRepository<Account> accountsRepo,
			IEfRepository<Transaction> transactionsRepo,
			IEfRepository<Category> categoriesRepo,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.usersRepo = usersRepo;
			this.accountsRepo = accountsRepo;
			this.transactionsRepo = transactionsRepo;
			this.categoriesRepo = categoriesRepo;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		public async Task<IEnumerable<string>> GetAdminsIdsAsync()
		{
			return await this.usersRepo.All()
				.Where(u => u.IsAdmin)
				.Select(u => u.Id.ToString().ToLower()) // .ToString().ToLower() because of SignalR Hubs
				.ToListAsync();
		}

		public async Task<AccountsAndCategoriesDropdownDTO> GetUserAccountsAndCategoriesDropdownsAsync(Guid userId)
		{
			string cacheKey = AccountsAndCategoriesKey + userId;

			if (!this.memoryCache.TryGetValue(cacheKey, out AccountsAndCategoriesDropdownDTO dropdowns))
			{
				dropdowns = await this.usersRepo.All()
					.Where(u => u.Id == userId)
					.ProjectTo<AccountsAndCategoriesDropdownDTO>(this.mapper.ConfigurationProvider)
					.FirstAsync();

				this.memoryCache.Set(cacheKey, dropdowns, TimeSpan.FromDays(3));
			}

			return dropdowns;
		}

		public async Task<IEnumerable<AccountCardDTO>> GetUserAccountsCardsAsync(Guid userId)
		{
			return await this.accountsRepo.All()
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => this.mapper.Map<AccountCardDTO>(a))
				.ToArrayAsync();
		}

		public async Task<AccountTypesAndCurrenciesDropdownDTO> GetUserAccountTypesAndCurrenciesDropdownsAsync(Guid userId)
		{
			string cacheKey = AccountTypesAndCurrenciesKey + userId;

			if (!this.memoryCache.TryGetValue(cacheKey, out AccountTypesAndCurrenciesDropdownDTO dropdowns))
			{
				dropdowns = await this.usersRepo.All()
					.Where(u => u.Id == userId)
					.ProjectTo<AccountTypesAndCurrenciesDropdownDTO>(this.mapper.ConfigurationProvider)
					.FirstAsync();

				this.memoryCache.Set(cacheKey, dropdowns, TimeSpan.FromDays(3));
			}

			return dropdowns;
		}

		public async Task<UserDashboardDTO> GetUserDashboardDataAsync(Guid userId, DateTime fromLocalTime, DateTime toLocalTime)
		{
			DateTime fromUtc = fromLocalTime.ToUniversalTime();
			DateTime toUtc = toLocalTime.ToUniversalTime();

			Expression<Func<Transaction, bool>> dateFilter = (t) 
				=> t.CreatedOnUtc >= fromUtc && t.CreatedOnUtc <= toUtc;

			UserDashboardDTO dto = await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.Select(u => new UserDashboardDTO
				{
					FromLocalTime = fromLocalTime,
					ToLocalTime = toLocalTime,
					Accounts = u.Accounts
						.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)
						.Select(a => new AccountCardDTO
						{
							Id = a.Id,
							Name = a.Name,
							Balance = a.Balance,
							CurrencyName = a.Currency.Name,
							OwnerId = a.OwnerId,
						}),
					CurrenciesCashFlow = u.Transactions
						.AsQueryable()
						.Where(dateFilter)
						.GroupBy(t => t.Account.Currency.Name)
						.Select(gr => new CurrencyCashFlowWithExpensesByCategoriesDTO
						{
							Name = gr.Key,
							Incomes = gr
								.Where(t => t.TransactionType == TransactionType.Income)
								.Sum(t => t.Amount),
							Expenses = gr
								.Where(t => t.TransactionType == TransactionType.Expense)
								.Sum(t => t.Amount),
							ExpensesByCategories = gr
								.Where(t => t.TransactionType == TransactionType.Expense)
								.GroupBy(t => t.Category.Name)
								.Select(subGr => new CategoryExpensesDTO
								{
									CategoryName = subGr.Key,
									ExpensesAmount = subGr.Sum(t => t.Amount)
								})
						})
						.ToArray(),
					LastTransactions = u.Transactions
						.AsQueryable()
						.Where(dateFilter)
						.OrderByDescending(t => t.CreatedOnUtc)
						.Take(5)
						.Select(t => new TransactionTableDTO
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = t.Account.Currency.Name +
								(t.Account.Currency.IsDeleted ? " (Deleted)" : string.Empty),
							CategoryName = t.Category.Name +
								(t.Category.IsDeleted ? " (Deleted)" : string.Empty),
							CreatedOnLocalTime = t.CreatedOnUtc.ToLocalTime(),
							Reference = t.Reference,
							TransactionType = t.TransactionType.ToString(),
						})
						.ToArray()
				})
				.AsSplitQuery()
				.FirstAsync();

			return dto;
		}

		public async Task<UserUsedDropdownsDTO> GetUserUsedDropdownsAsync(Guid userId)
		{
			UserUsedDropdownsDTO resultDTO = await this.usersRepo.All()
				.Where(u => u.Id == userId)
				.ProjectTo<UserUsedDropdownsDTO>(this.mapper.ConfigurationProvider)
				.FirstAsync();

			resultDTO.OwnerCategories.Add(await this.categoriesRepo.All()
				.Where(c => c.Id == Guid.Parse(InitialBalanceCategoryId))
				.ProjectTo<DropdownDTO>(this.mapper.ConfigurationProvider)
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