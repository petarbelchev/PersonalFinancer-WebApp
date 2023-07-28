#nullable disable

namespace PersonalFinancer.Services.Users.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class UserUsedDropdownsDTO
	{
		public IEnumerable<DropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; set; }

		public ICollection<DropdownDTO> OwnerCategories { get; set; }
	}
}
