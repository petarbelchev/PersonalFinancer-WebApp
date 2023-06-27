#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Data.Models.Enums;
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditTransactionDTO
	{
		public decimal Amount { get; set; }
		
		public Guid AccountId { get; set; }
		
		public DateTime CreatedOn { get; set; }
		
		public Guid CategoryId { get; set; }

		public Guid OwnerId { get; set; }
				
		public string Reference { get; set; }

		public TransactionType TransactionType { get; set; }
		
		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; set; }
	}
}
