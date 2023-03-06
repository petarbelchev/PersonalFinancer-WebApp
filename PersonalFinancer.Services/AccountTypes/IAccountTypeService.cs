using PersonalFinancer.Services.AccountTypes.Models;

namespace PersonalFinancer.Services.AccountTypes
{
	public interface IAccountTypeService
	{
		/// <summary>
		/// Returns collection of Account Types for the current user with Id and Name.
		/// </summary>
		Task<IEnumerable<AccountTypeViewModel>> AccountTypesViewModel(string userId);

		/// <summary>
		/// Creates new Account Type with given Name. If you try to create a new Account Type with name that other Account Type have, throws exception.
		/// </summary>
		/// <returns>View Model with Id, Name and User Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		Task<AccountTypeViewModel> CreateAccountType(string userId, string accountTypeName);

		/// <summary>
		/// Delete Account Type with given Id or throws exception when Account Type does not exist or User is not owner.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccountType(Guid categoryId, string userId);
	}
}
