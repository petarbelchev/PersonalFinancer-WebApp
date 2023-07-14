#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class UserUsedDropdownsDTO
	{
		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }

		public ICollection<CategoryDropdownDTO> OwnerCategories { get; set; }
	}
}
