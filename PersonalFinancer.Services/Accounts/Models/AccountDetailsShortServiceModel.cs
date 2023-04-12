namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsShortServiceModel
	{
		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;
	}
}
