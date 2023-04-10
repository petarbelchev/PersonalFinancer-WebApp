namespace PersonalFinancer.Web.Models.Accounts
{
	using Web.Models.Shared;

	public interface IAccountFormModel
	{
		public string Name { get; set; }

		public decimal? Balance { get; set; }

		public string OwnerId { get; set; }

		public string AccountTypeId { get; set; }

		public IEnumerable<AccountTypeViewModel> AccountTypes { get; set; }

		public string CurrencyId { get; set; }

		public IEnumerable<CurrencyViewModel> Currencies { get; set; }
	}
}
