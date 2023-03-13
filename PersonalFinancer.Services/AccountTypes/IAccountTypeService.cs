using PersonalFinancer.Services.AccountTypes.Models;

namespace PersonalFinancer.Services.AccountTypes
{
	public interface IAccountTypeService
	{
		Task<IEnumerable<AccountTypeViewModel>> GetUserAccountTypesViewModel(string userId);
		
		/// <summary>
		/// Throws ArgumentException if given name already exists or name length is invalid.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task CreateAccountType(string userId, AccountTypeViewModel model);
		
		/// <summary>
		/// Throws exception when Account Type does not exist or User is not owner.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccountType(Guid categoryId, string userId);
	}
}
