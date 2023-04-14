namespace PersonalFinancer.Services.User.Models
{
	using Services.Shared.Models;

	public class UserAccountTypesAndCurrenciesServiceModel
	{
		public IEnumerable<AccountTypeServiceModel> AccountTypes { get; set; } = null!;

		public IEnumerable<CurrencyServiceModel> Currencies { get; set; } = null!;
	}
}
