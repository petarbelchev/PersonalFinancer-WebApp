#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditTransactionOutputDTO : CreateEditTransactionInputDTO
	{		
		public IEnumerable<DropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<DropdownDTO> OwnerCategories { get; set; }
	}
}
