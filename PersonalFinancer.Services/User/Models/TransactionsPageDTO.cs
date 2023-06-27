#nullable disable

namespace PersonalFinancer.Services.User.Models
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;

	public class TransactionsPageDTO : TransactionsDTO
	{
		public IEnumerable<AccountDropdownDTO> UserAccounts { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> UserAccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> UserCurrencies { get; set; }

		public IEnumerable<CategoryDropdownDTO> UserCategories { get; set; }
	}
}
