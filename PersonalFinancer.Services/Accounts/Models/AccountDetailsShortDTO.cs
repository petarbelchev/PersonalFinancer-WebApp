#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
    public class AccountDetailsShortDTO
    {
        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string CurrencyName { get; set; }

		public string AccountTypeName { get; set; }
	}
}
