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
		Task<AccountTypeViewModel> CreateAccountType(AccountTypeInputModel model);
		
		/// <summary>
		/// Throws exception when Account Type does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccountType(string accountTypeId, string? ownerId = null);
	}
}
