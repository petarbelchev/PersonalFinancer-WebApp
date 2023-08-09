#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsDTO : IHaveOwner
    {
        public Guid Id { get; set; }

		public string Name { get; set; }

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; }

		public string AccountTypeName { get; set; }

		public Guid OwnerId { get; set; }
    }
}
