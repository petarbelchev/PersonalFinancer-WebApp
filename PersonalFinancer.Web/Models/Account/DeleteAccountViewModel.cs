namespace PersonalFinancer.Web.Models.Account
{
	public class DeleteAccountViewModel
	{
		public string Name { get; set; } = null!;

		public bool ShouldDeleteTransactions { get; set; }
	}
}
