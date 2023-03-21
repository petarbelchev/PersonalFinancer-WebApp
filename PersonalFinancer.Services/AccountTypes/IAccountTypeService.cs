using PersonalFinancer.Services.AccountTypes.Models;

namespace PersonalFinancer.Services.AccountTypes
{
	public interface IAccountTypeService
	{
		Task<IEnumerable<AccountTypeViewModel>> GetUserAccountTypesViewModel(string userId);
		
		/// <summary>
		/// Throws ArgumentException if given name already exists.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<AccountTypeViewModel> CreateAccountType(string userId, string accountTypeName);
		
		/// <summary>
		/// Throws exception when Account Type does not exist
		/// and ArgumentException when User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccountType(string categoryId, string userId);
	}
}
