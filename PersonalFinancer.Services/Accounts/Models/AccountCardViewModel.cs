namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountCardViewModel
	{
		public string Id { get; set; } = null!;

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;
	}
}
