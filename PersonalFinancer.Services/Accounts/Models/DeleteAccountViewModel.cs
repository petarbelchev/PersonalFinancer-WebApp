namespace PersonalFinancer.Services.Accounts.Models
{
	public class DeleteAccountViewModel
	{
		public string Name { get; set; } = null!;

		public bool ShouldDeleteTransactions { get; set; }
	}
}
