#nullable disable

namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class UserTransactionsViewModel : UserTransactionsInputModel
	{
		public UserTransactionsViewModel(
			UserTransactionsInputModel filter,
			UserDropdownsDTO dropdowns,
			Guid userId)
			: this(dropdowns)
		{
			this.Pagination = new PaginationModel(
				TransactionsName,
				TransactionsPerPage,
				totalElements: 0);

			this.AccountId = filter.AccountId;
			this.AccountTypeId = filter.AccountTypeId;
			this.CurrencyId = filter.CurrencyId;
			this.CategoryId = filter.CategoryId;
			this.FromLocalTime = filter.FromLocalTime;
			this.ToLocalTime = filter.ToLocalTime;
			this.UserId = userId;
			this.Transactions = new List<TransactionTableDTO>();
		}

		public UserTransactionsViewModel(
			TransactionsFilterDTO filter,
			UserDropdownsDTO dropdowns,
			TransactionsDTO transactions)
			: this(dropdowns)
		{
			this.Pagination = new PaginationModel(
				TransactionsName,
				TransactionsPerPage,
				transactions.TotalTransactionsCount);

			this.AccountId = filter.AccountId;
			this.AccountTypeId = filter.AccountTypeId;
			this.CurrencyId = filter.CurrencyId;
			this.CategoryId = filter.CategoryId;
			this.FromLocalTime = filter.FromLocalTime;
			this.ToLocalTime = filter.ToLocalTime;
			this.UserId = filter.UserId;
			this.Transactions = transactions.Transactions;
		}

		private UserTransactionsViewModel(UserDropdownsDTO dropdowns)
		{
			this.OwnerAccounts = dropdowns.OwnerAccounts;
			this.OwnerAccountTypes = dropdowns.OwnerAccountTypes;
			this.OwnerCurrencies = dropdowns.OwnerCurrencies;
			this.OwnerCategories = dropdowns.OwnerCategories;
		}

		public Guid UserId { get; set; }

		public IEnumerable<TransactionTableDTO> Transactions { get; private set; }

		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; private set; }

		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; private set; }

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; private set; }

		public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; private set; }

		public PaginationModel Pagination { get; private set; }
	}
}
