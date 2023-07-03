#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class UserDropdownDTO
	{
		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }

		public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; set; }
	}
}
