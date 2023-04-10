namespace PersonalFinancer.Services.Shared.Models
{
	using Services.Accounts.Models;
	using Services.Categories.Models;

	public class EmptyTransactionFormDTO
	{
		public string OwnerId { get; set; } = null!;

		public DateTime CreatedOn { get; set; }

		public IEnumerable<AccountDTO> UserAccounts { get; set; } = null!;

		public IEnumerable<CategoryOutputDTO> UserCategories { get; set; } = null!;
	}
}
