#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditAccountOutputDTO : CreateEditAccountInputDTO
	{
		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; set; }
	}
}
