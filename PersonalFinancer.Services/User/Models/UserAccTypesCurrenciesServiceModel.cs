using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Currencies.Models;

namespace PersonalFinancer.Services.User.Models
{
	public class UserAccTypesCurrenciesServiceModel
	{
		public IEnumerable<AccountTypeViewModel> AccountTypes { get; set; } = null!;

        public IEnumerable<CurrencyViewModel> Currencies { get; set; } = null!;
    }
}
