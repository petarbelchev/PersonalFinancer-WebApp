using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
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

		public async Task<AllUsersViewModel> GetAllUsers(int page)
		{
			var model = new AllUsersViewModel();
			model.Pagination.Page = page;
			model.Pagination.TotalElements = data.Users.Count();
			model.Users = await data.Users
				.OrderBy(u => u.FirstName)
				.ThenBy(u => u.LastName)
				.Skip(model.Pagination.ElementsPerPage * (model.Pagination.Page - 1))
				.Take(model.Pagination.ElementsPerPage)
				.ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			return model;
		}

		public async Task<IEnumerable<AccountCardViewModel>> GetUserAccounts(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountCardViewModel>(a))
				.ToArrayAsync();
		}

		public int GetUsersAccountsCount()
		{
			int accountsCount = data.Accounts.Count(a => !a.IsDeleted);

			return accountsCount;
		}

		public async Task SetUserDashboard(string userId, UserDashboardViewModel model)
		{
			var dto = await data.Users
				.Where(u => u.Id == userId)
				.Select(u => new UserDashboardDTO
				{
					Accounts = u.Accounts.Where(a => !a.IsDeleted)
						.OrderBy(a => a.Name)
						.Select(a => new AccountCardViewModel
						{
							Id = a.Id,
							Name = a.Name,
							Balance = a.Balance,
							CurrencyName = a.Currency.Name
						}),
					LastTransactions = u.Transactions
						.Where(t => t.CreatedOn >= model.StartDate
									&& t.CreatedOn <= model.EndDate)
						.OrderByDescending(t => t.CreatedOn)
						.Take(5)
						.Select(t => new TransactionTableViewModel
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
						.GroupBy(t => t.Account.Currency.Name)
						.Select(t => new CurrencyCashFlowViewModel
						{
							Name = t.Key,
							Incomes = t.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
							Expenses = t.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
						})
						.OrderBy(c => c.Name)
						.ToList()
				})
				.FirstAsync();

			model.Accounts = dto.Accounts;
			model.Transactions = dto.LastTransactions;
			model.CurrenciesCashFlow = dto.CurrenciesCashFlow;
		}

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserDetailsViewModel> UserDetails(string userId)
		{
			var result = await data.Users
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsViewModel>(mapper.ConfigurationProvider)
				.FirstAsync();

			return result;
		}

		public int UsersCount() => data.Users.Count();
	}
}