#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class AccountsAndCategoriesDropdownDTO
	{
		public IEnumerable<AccountDropdownDTO> UserAccounts { get; set; }

		public IEnumerable<CategoryDropdownDTO> UserCategories { get; set; }
	}
}
