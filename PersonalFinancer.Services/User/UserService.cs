namespace PersonalFinancer.Services.User
{
	using Microsoft.EntityFrameworkCore;
	using AutoMapper;
	using AutoMapper.QueryableExtensions;

	using Models;
	using Accounts;
	using Accounts.Models;
	using Data;
	using Data.Models;
	using Transactions;

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

		/// <summary>
		/// Returns collection of all users 
		/// </summary>
		public async Task<IEnumerable<UserViewModel>> All()
		{
			IEnumerable<UserViewModel> users = await data.Users
				.ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			return users;
		}

		/// <summary>
		/// Returns Dashboard View Model for current User with Last transactions, Accounts and Currencies Cash Flow.
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		public async Task GetUserDashboard(string userId, DashboardServiceModel model)
		{
			model.Accounts = await accountService.AllAccountsCardViewModel(userId);

			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			model.LastTransactions = await transactionsService
				.LastFiveTransactions(userId, model.StartDate, model.EndDate);

			model.CurrenciesCashFlow = await accountService
				.GetUserAccountsCashFlow(userId, model.StartDate, model.EndDate);
		}

		/// <summary>
		/// Returns count of all registered users.
		/// </summary>
		public int UsersCount()
		{
			int usersCount = data.Users.Count();

			return usersCount;
		}

		/// <summary>
		/// Returns User Details View Model used for Admin User Details page
		/// </summary>
		public async Task<UserDetailsViewModel> UserDetails(string userId)
		{
			UserDetailsViewModel user = await data.Users
				.Where(u => u.Id == userId)
				.ProjectTo<UserDetailsViewModel>(mapper.ConfigurationProvider)
				.FirstAsync();

			return user;
		}

		/// <summary>
		/// Returns User's full name or throws exception when user does not exist.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<string> FullName(string userId)
		{
			ApplicationUser? user = await data.Users.FindAsync(userId);

			if (user == null)
			{
				throw new ArgumentNullException("User does not exist.");
			}

			return $"{user.FirstName} {user.LastName}";
		}
	}
}
