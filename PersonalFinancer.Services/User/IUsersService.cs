namespace PersonalFinancer.Services.User
{
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User.Models;

    public interface IUsersService
    {
        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<string> UserFullName(Guid userId);

        Task<UsersServiceModel> GetAllUsers(int page);

        Task<UserAccountsAndCategoriesServiceModel> GetUserAccountsAndCategories(Guid? userId);

        Task<UserAccountTypesAndCurrenciesServiceModel> GetUserAccountTypesAndCurrencies(Guid? userId);

        Task<IEnumerable<AccountCardServiceModel>> GetUserAccounts(Guid userId);

        Task<int> GetUsersAccountsCount();

        Task<TransactionsServiceModel> GetUserTransactions(Guid? userId, DateTime startDate, DateTime endDate, int page = 1);

        Task<UserDashboardServiceModel> GetUserDashboardData(Guid userId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<UserDetailsServiceModel> UserDetails(Guid? userId);

        Task<int> UsersCount();
    }
}
