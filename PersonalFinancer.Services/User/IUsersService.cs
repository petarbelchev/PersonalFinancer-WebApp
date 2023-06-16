namespace PersonalFinancer.Services.User
{
    using PersonalFinancer.Services.User.Models;

    public interface IUsersService
    {
        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<string> UserFullNameAsync(Guid userId);

        Task<UsersServiceModel> GetAllUsersAsync(int page);

        Task<UserAccountsAndCategoriesServiceModel> GetUserAccountsAndCategoriesAsync(Guid userId);

        Task<UserAccountTypesAndCurrenciesServiceModel> GetUserAccountTypesAndCurrenciesAsync(Guid userId);

        Task<UserDashboardServiceModel> GetUserDashboardDataAsync(Guid userId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<UserDetailsServiceModel> UserDetailsAsync(Guid userId);

        Task<int> UsersCountAsync();
    }
}
