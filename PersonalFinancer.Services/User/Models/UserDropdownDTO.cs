#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class UserDropdownDTO
	{
		public IEnumerable<AccountDropdownDTO> Accounts { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> AccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> Currencies { get; set; }

		public IEnumerable<CategoryDropdownDTO> Categories { get; set; }
	}
}
