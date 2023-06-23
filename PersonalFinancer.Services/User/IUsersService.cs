namespace PersonalFinancer.Services.User
{
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;

    public interface IUsersService
    {
        Task<UsersServiceModel> GetAllUsersAsync(int page);

        Task<IEnumerable<AccountServiceModel>> GetUserAccountsDropdownData(Guid userId, bool withDeleted);

        Task<IEnumerable<AccountTypeServiceModel>> GetUserAccountTypesDropdownData(Guid userId, bool withDeleted);
		
        Task<IEnumerable<CategoryServiceModel>> GetUserCategoriesDropdownData(Guid userId, bool withDeleted);

        Task<IEnumerable<CurrencyServiceModel>> GetUserCurrenciesDropdownData(Guid userId, bool withDeleted);

		Task<UserDashboardServiceModel> GetUserDashboardDataAsync(Guid userId, DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<UserDetailsServiceModel> UserDetailsAsync(Guid userId);

        /// <summary>
        /// Throws InvalidOperationException if User does not exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task<string> UserFullNameAsync(Guid userId);

        Task<int> UsersCountAsync();
    }
}
