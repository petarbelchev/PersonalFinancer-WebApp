namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountFormShortServiceModel
	{
		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string OwnerId { get; set; } = null!;

		public string AccountTypeId { get; set; } = null!;

		public string CurrencyId { get; set; } = null!;
	}
}
