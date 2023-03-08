using PersonalFinancer.Services.Currencies.Models;

namespace PersonalFinancer.Services.Currencies
{
	public interface ICurrencyService
	{
		/// <summary>
		/// Creates new Currency with given Name. 
		/// If you try to create a new Currency with name that other Currency have, or name length is invalid, throws exception.
		/// </summary>
		/// <returns>View Model with Id, Name and User Id.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CurrencyViewModel> CreateCurrency(string userId, string currencyName);

		/// <summary>
		/// Delete Currency with given Id or throws exception when Currency does not exist or User is not owner.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCurrency(Guid currencyId, string userId);

		/// <summary>
		/// Returns collection of User's currencies with props: Id and Name.
		/// </summary>
		Task<IEnumerable<CurrencyViewModel>> UserCurrencies(string userId);
	}
}
