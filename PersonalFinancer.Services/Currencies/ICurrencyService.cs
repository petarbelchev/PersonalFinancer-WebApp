using PersonalFinancer.Services.Currencies.Models;

namespace PersonalFinancer.Services.Currencies
{
	public interface ICurrencyService
	{
		/// <summary>
		/// Throws ArgumentException if given name exists.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<CurrencyViewModel> CreateCurrency(CurrencyInputModel model);
		
		/// <summary>
		/// Throws InvalidOperationException when Currency does not exist
		/// and ArgumentException when User is not owner.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCurrency(string currencyId, string userId);

		Task<IEnumerable<CurrencyViewModel>> GetUserCurrencies(string userId);
	}
}
