namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Users.Models;

	public class UserTransactionsViewModel : UserTransactionsInputModel
	{
		public UserTransactionsViewModel(
			UserTransactionsInputModel filter,
			UserUsedDropdownsDTO dropdowns,
			Guid userId)
		{
			this.OwnerAccounts = dropdowns.OwnerAccounts;
			this.OwnerAccountTypes = dropdowns.OwnerAccountTypes;
			this.OwnerCurrencies = dropdowns.OwnerCurrencies;
			this.OwnerCategories = dropdowns.OwnerCategories;

			this.AccountId = filter.AccountId;
			this.AccountTypeId = filter.AccountTypeId;
			this.CurrencyId = filter.CurrencyId;
			this.CategoryId = filter.CategoryId;
			this.FromLocalTime = filter.FromLocalTime;
			this.ToLocalTime = filter.ToLocalTime;

			this.UserId = userId;
		}

		public Guid UserId { get; private set; }

		public IEnumerable<DropdownDTO> OwnerAccounts { get; private set; }

		public IEnumerable<DropdownDTO> OwnerAccountTypes { get; private set; }

		public IEnumerable<DropdownDTO> OwnerCurrencies { get; private set; }

		public IEnumerable<DropdownDTO> OwnerCategories { get; private set; }
	}
}
