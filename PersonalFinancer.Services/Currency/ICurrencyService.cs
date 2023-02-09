using PersonalFinancer.Services.Currency.Models;

namespace PersonalFinancer.Services.Currency
{
	public interface ICurrencyService
	{
		Task<IEnumerable<CurrencyViewModel>> AllCurrencies(string userId);
	}
}
