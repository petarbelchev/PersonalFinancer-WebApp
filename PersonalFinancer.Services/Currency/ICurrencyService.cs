namespace PersonalFinancer.Services.Currency
{
	using Models;

	public interface ICurrencyService
	{
		Task<IEnumerable<CurrencyViewModel>> AllCurrencies(string userId);
	}
}
