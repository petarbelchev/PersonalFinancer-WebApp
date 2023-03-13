using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.User
{
    public class UserService : IUserService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IAccountService accountService;
		private readonly ITransactionsService transactionsService;
		private readonly IMapper mapper;

		public UserService(
			PersonalFinancerDbContext data,
			IAccountService accountService,
			ITransactionsService transactionsService,
			IMapper mapper)
		{
			this.data = data;
			this.accountService = accountService;
			this.transactionsService = transactionsService;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<UserViewModel>> All()
		{
			return await data.Users
				.ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();
		}
		
		/// <summary>
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public async Task GetUserDashboard(string userId, HomeIndexViewModel model)
		{
			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			model.Accounts = await accountService.GetUserAccountCardsViewModel(userId);

			model.LastTransactions = await transactionsService
				.GetUserLastFiveTransactions(userId, model.StartDate, model.EndDate);

			model.CurrenciesCashFlow = await accountService
				.GetUserAccountsCashFlow(userId, model.StartDate, model.EndDate);
		}

		public int UsersCount() => data.Users.Count();

		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<UserDetailsViewModel> UserDetails(string userId)
		{
			return await data.Users
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsViewModel>(mapper.ConfigurationProvider)
				.FirstAsync();
		}
		
		/// <summary>
		/// Throws InvalidOperationException if User does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<string> FullName(string userId)
		{
			ApplicationUser? user = await data.Users.FindAsync(userId);

			if (user == null)
			{
				throw new InvalidOperationException("User does not exist.");
			}

			return $"{user.FirstName} {user.LastName}";
		}
	}
}
