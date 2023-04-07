using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class CreateAccountFormDTO
	{
        public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string OwnerId { get; set; } = null!;

		public string AccountTypeId { get; set; } = null!;

		public IEnumerable<AccountTypeOutputDTO> AccountTypes { get; set; } = null!;

		public string CurrencyId { get; set; } = null!;

		public IEnumerable<CurrencyOutputDTO> Currencies { get; set; } = null!;
	}
}
