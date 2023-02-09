namespace PersonalFinancer.Services.Account.Models
{
	public class AccountViewModelExtended
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string Currency { get; set; } = null!;
	}
}
