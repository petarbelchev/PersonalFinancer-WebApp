namespace PersonalFinancer.Services.User
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;

	using Microsoft.EntityFrameworkCore;

	using Data;
	using Data.Enums;
	using Data.Models;

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

		public async Task<AllUsersDTO> GetAllUsers(int page, int elementsPerPage)
		{
			return new AllUsersDTO
			{
				Users = await data.Users
					.OrderBy(u => u.FirstName)
					.ThenBy(u => u.LastName)
					.Skip(elementsPerPage * (page - 1))
					.Take(elementsPerPage)
					.Select(u => mapper.Map<UserDTO>(u))
					.ToListAsync(),
				Page = page,
				AllUsersCount = data.Users.Count(),
			};
		}

		public async Task<IEnumerable<AccountCardDTO>> GetUserAccounts(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountCardDTO>(a))
				.ToArrayAsync();
		}

		public int GetUsersAccountsCount()
		{
			int accountsCount = data.Accounts.Count(a => !a.IsDeleted);

			return accountsCount;
		}

		public async Task<UserDashboardDTO> GetUserDashboardData(string userId, DateTime startDate, DateTime endDate)
		{
			return await data.Users
				.Where(u => u.Id == userId)
				.Select(u => new UserDashboardDTO
				{
					StartDate = startDate,
					EndDate = endDate,
					Accounts = u.Accounts.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)
						.Select(a => new AccountCardDTO
						{
							Id = a.Id,
							Name = a.Name,
							Balance = a.Balance,
							CurrencyName = a.Currency.Name
						}),
					Transactions = u.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.OrderByDescending(t => t.CreatedOn)
						.Take(5)
						.Select(t => new TransactionTableDTO
						{
							Id = t.Id,
							Amount = t.Amount,
							AccountCurrencyName = t.Account.Currency.Name,
							CategoryName = t.Category.Name,
							CreatedOn = t.CreatedOn,
							Refference = t.Refference,
							TransactionType = t.TransactionType.ToString()
						}),
					CurrenciesCashFlow = u.Transactions
						.Where(t => t.CreatedOn >= startDate && t.CreatedOn <= endDate)
						.GroupBy(t => t.Account.Currency.Name)
						.Select(t => new CurrencyCashFlowDTO
						{
							Name = t.Key,
							Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
							Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
						})
						.OrderBy(c => c.Name)
						.ToList()
				})
				.FirstAsync();
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserDetailsDTO> UserDetails(string userId)
		{
			var result = await data.Users
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsDTO>(mapper.ConfigurationProvider)
				.FirstAsync();

			return result;
		}

		public int UsersCount() => data.Users.Count();
	}
}