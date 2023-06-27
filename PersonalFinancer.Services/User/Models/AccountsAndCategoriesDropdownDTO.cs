#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class AccountsAndCategoriesDropdownDTO
	{
		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; set; }
	}
}
