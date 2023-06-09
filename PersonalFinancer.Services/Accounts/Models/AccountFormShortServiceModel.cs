namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountFormShortServiceModel
	{
		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public Guid OwnerId { get; set; }

		public Guid AccountTypeId { get; set; }

		public Guid CurrencyId { get; set; }
	}
}
