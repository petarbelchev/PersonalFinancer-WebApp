#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class AccountsAndCategoriesDropdownDTO
	{
		public IEnumerable<DropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<DropdownDTO> OwnerCategories { get; set; }
	}
}
