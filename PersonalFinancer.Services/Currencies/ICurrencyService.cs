using PersonalFinancer.Services.Currencies.Models;

namespace PersonalFinancer.Services.Currencies
{
	public interface ICurrencyService
	{
		/// <summary>
		/// Throws ArgumentException if given name exists or lenght is invalid.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task CreateCurrency(string userId, CurrencyViewModel model);
		
		/// <summary>
		/// Throws InvalidOperationException when Currency does not exist or User is not owner.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCurrency(string currencyId, string userId);

		Task<IEnumerable<CurrencyViewModel>> GetUserCurrencies(string userId);
	}
}
