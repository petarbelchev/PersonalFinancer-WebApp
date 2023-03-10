namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountCardViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;
	}
}
