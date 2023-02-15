namespace PersonalFinancer.Services.Currency
{
	using Models;

	public interface ICurrencyService
	{
		/// <summary>
		/// Returns collection of User's currencies with props: Id and Name.
		/// </summary>
		Task<IEnumerable<CurrencyViewModel>> UserCurrencies(string userId);
	}
}
