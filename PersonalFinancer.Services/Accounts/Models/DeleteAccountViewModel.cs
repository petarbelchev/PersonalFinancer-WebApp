namespace PersonalFinancer.Services.Accounts.Models
{
	public class DeleteAccountViewModel : AccountDropdownViewModel
	{
		public string OwnerId { get; set; } = null!;

		public bool ShouldDeleteTransactions { get; set; }

		public string ReturnUrl { get; set; } = "~/Home/Index";
	}
}
