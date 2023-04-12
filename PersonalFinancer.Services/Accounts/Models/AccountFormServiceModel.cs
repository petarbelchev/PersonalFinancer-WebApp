namespace PersonalFinancer.Services.Accounts.Models
{
	using Services.Shared.Models;

	public class AccountFormServiceModel : AccountFormShortServiceModel
	{
		public IEnumerable<CurrencyServiceModel> Currencies { get; set; } = null!;

		public IEnumerable<AccountTypeServiceModel> AccountTypes { get; set; } = null!;
	}
}
