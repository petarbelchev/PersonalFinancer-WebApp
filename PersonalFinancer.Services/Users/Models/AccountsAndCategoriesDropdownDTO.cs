#nullable disable

namespace PersonalFinancer.Services.Users.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class AccountsAndCategoriesDropdownDTO
	{
		public IEnumerable<DropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<DropdownDTO> OwnerCategories { get; set; }
	}
}
