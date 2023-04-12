namespace PersonalFinancer.Services.User
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;

    using Data;
    using Data.Enums;
    using Data.Models;
    using static Data.Constants;

    using Services.Accounts.Models;
    using Services.Shared.Models;
    using Services.User.Models;

    public class UsersService : IUsersService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;

		public UsersService(
			PersonalFinancerDbContext data,
			IMapper mapper)
		{
			this.data = data;
			this.mapper = mapper;
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> FullName(string userId)
		{
			ApplicationUser? user = await data.Users.FindAsync(userId);

			if (user == null)
				throw new InvalidOperationException("User does not exist.");

			return $"{user.FirstName} {user.LastName}";
		}

		public async Task<UsersServiceModel> GetAllUsers(int page)
		{
			var users = new UsersServiceModel
			{
				Users = await data.Users
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Skip(PaginationConstants.UsersPerPage * (page - 1))
					.Take(PaginationConstants.UsersPerPage)
					.ProjectTo<UserServiceModel>(mapper.ConfigurationProvider)
					.ToListAsync(),
				TotalUsersCount = data.Users.Count()
			};

			return users;
		}

		public async Task<UserAccountsAndCategoriesServiceModel> GetUserAccountsAndCategories(string userId)
		{
			var userData = await data.Users.Where(u => u.Id == userId)
				.Select(u => new UserAccountsAndCategoriesServiceModel
				{
					UserAccounts = u.Accounts
						.Where(a => !a.IsDeleted)
						.Select(a => mapper.Map<AccountServiceModel>(a)),
					UserCategories = u.Categories
						.Where(c => !c.IsDeleted)
						.Select(c => mapper.Map<CategoryServiceModel>(c))
				})
				.FirstAsync();

			return userData;
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserAccountTypesAndCurrenciesServiceModel> GetUserAccountTypesAndCurrencies(string userId)
		{
			return await data.Users.Where(u => u.Id == userId)
				.Select(u => new UserAccountTypesAndCurrenciesServiceModel
				{
					AccountTypes = u.AccountTypes
						.Where(at => !at.IsDeleted)
						.Select(at => new AccountTypeServiceModel
						{
							Id = at.Id,
							Name = at.Name
						}),
					Currencies = u.Currencies
						.Where(c => !c.IsDeleted)
						.Select(c => new CurrencyServiceModel
						{
							Id = c.Id,
							Name = c.Name
						})
				})
				.FirstAsync();
		}

		public async Task<IEnumerable<AccountCardServiceModel>> GetUserAccounts(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountCardServiceModel>(a))
				.ToArrayAsync();
		}

		public int GetUsersAccountsCount()
		{
			int accountsCount = data.Accounts.Count(a => !a.IsDeleted);

			return accountsCount;
		}

		public async Task<TransactionsServiceModel> GetUserTransactions(string userId, DateTime startDate, DateTime endDate, int page = 1)
		{
			TransactionsServiceModel userTransactions = await data.Users
				.Where(u => u.Id == userId)
				.Select(u => new TransactionsServiceModel
				{
					TotalTransactionsCount = u.Transactions
						.Count(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate),
					Transactions = u.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Skip(PaginationConstants.TransactionsPerPage * (page - 1))
						.Take(PaginationConstants.TransactionsPerPage)
						.Select(t => new TransactionTableServiceModel
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
						})
				})
				.FirstAsync();

			return userTransactions;
		}

		public async Task<UserDashboardServiceModel> GetUserDashboardData(string userId, DateTime startDate, DateTime endDate)
		{
			var dto = await data.Users
				.Where(u => u.Id == userId)
				.Select(u => new UserDashboardServiceModel
				{
					Accounts = u.Accounts.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)
						.Select(a => new AccountCardServiceModel
						{
							Id = a.Id,
							Name = a.Name,
							Balance = a.Balance,
							CurrencyName = a.Currency.Name
						}),
					LastTransactions = u.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Take(5)
						.Select(t => new TransactionTableServiceModel
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = t.Account.Currency.Name,
							CreatedOn = t.CreatedOn,
							TransactionType = t.TransactionType.ToString(),
							Refference = t.Refference,
							CategoryName = t.Category.Name
						}),
					CurrenciesCashFlow = u.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.GroupBy(t => t.Account.Currency.Name)
						.Select(t => new CurrencyCashFlowServiceModel
						{
							Name = t.Key,
							Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
							Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
						})
						.OrderBy(c => c.Name)
						.ToList()
				})
				.FirstAsync();

			return dto;
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserDetailsServiceModel> UserDetails(string userId)
		{
			var result = await data.Users
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsServiceModel>(mapper.ConfigurationProvider)
				.FirstAsync();

			return result;
		}

		public int UsersCount() => data.Users.Count();
	}
}