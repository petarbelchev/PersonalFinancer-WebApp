namespace PersonalFinancer.Services.AccountTypes
{
	using Services.AccountTypes.Models;
	using Services.Shared.Models;
	
	public interface IAccountTypeService
	{
		/// <summary>
		/// Throws ArgumentException if given name already exists.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<AccountTypeOutputDTO> CreateAccountType(AccountTypeInputDTO model);

		/// <summary>
		/// Throws exception when Account Type does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteAccountType(string accountTypeId, string? ownerId = null);
	}
}
