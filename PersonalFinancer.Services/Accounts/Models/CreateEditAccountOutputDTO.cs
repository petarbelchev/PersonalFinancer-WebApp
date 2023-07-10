#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditAccountOutputDTO : CreateEditAccountInputDTO
	{
		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }
	}
}
