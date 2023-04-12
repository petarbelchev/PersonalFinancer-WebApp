using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.User.Models
{
	public class UserAccountTypesAndCurrenciesServiceModel
	{
		public IEnumerable<AccountTypeServiceModel> AccountTypes { get; set; } = null!;

        public IEnumerable<CurrencyServiceModel> Currencies { get; set; } = null!;
    }
}
