namespace PersonalFinancer.Web.Models.Accounts
{
	public class DeleteAccountViewModel
	{
		public string Name { get; set; } = null!;

		public bool ShouldDeleteTransactions { get; set; }
	}
}
