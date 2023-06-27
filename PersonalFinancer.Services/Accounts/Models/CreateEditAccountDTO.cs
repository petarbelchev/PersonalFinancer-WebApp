#nullable disable

namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class CreateEditAccountDTO
	{
		public string Name { get; set; }

		public decimal Balance { get; set; }

		public Guid OwnerId { get; set; }

		public Guid AccountTypeId { get; set; }

		public Guid CurrencyId { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }
	}
}
