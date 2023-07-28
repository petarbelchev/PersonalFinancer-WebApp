#nullable disable

namespace PersonalFinancer.Services.Users.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class AccountTypesAndCurrenciesDropdownDTO
	{
		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; set; }
	}
}
