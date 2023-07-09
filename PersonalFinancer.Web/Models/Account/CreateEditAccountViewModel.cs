namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditAccountViewModel : CreateEditAccountInputModel
	{
		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }
			= new List<AccountTypeDropdownDTO>();

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }
			= new List<CurrencyDropdownDTO>();
	}
}
