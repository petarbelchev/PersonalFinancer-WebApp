namespace PersonalFinancer.Web.Models.Account
{
	public class AccountDetailsViewModel : AccountDetailsInputModel
	{
		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;

		public string AccountTypeName { get; set; } = null!;

		public Guid OwnerId { get; set; }
    }
}
