#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class AccountTypesAndCurrenciesDropdownDTO
	{
		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; set; }
	}
}
