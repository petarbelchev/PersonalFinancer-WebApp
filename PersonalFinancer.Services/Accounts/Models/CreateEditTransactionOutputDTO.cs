#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditTransactionOutputDTO : CreateEditTransactionInputDTO
	{		
		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; set; }
	}
}
