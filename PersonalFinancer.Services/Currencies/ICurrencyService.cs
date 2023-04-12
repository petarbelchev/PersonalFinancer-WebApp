namespace PersonalFinancer.Services.Currencies
{
	using Services.Currencies.Models;
	using Services.Shared.Models;
	
	public interface ICurrencyService
	{
		/// <summary>
		/// Throws ArgumentException if given name exists.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<CurrencyServiceModel> CreateCurrency(CurrencyInputModel model);

		/// <summary>
		/// Throws InvalidOperationException when Currency does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCurrency(string currencyId, string? ownerId = null);
	}
}
