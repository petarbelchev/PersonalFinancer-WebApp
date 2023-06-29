namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class UserTransactionsViewModel : UserTransactionsInputModel
	{
		public UserTransactionsViewModel(int totalTransactionsCount, int page = 1)
		{
			this.Pagination = new PaginationModel(
				TransactionsName, TransactionsPerPage, totalTransactionsCount, page);

			this.Routing = new RoutingModel
			{
				Controller = "Transactions",
				Action = "All"
			};

			this.Transactions = new List<TransactionTableDTO>();
			this.OwnerAccounts = new List<AccountDropdownDTO>();
			this.OwnerAccountTypes = new List<AccountTypeDropdownDTO>();
			this.OwnerCurrencies = new List<CurrencyDropdownDTO>();
			this.OwnerCategories = new List<CategoryDropdownDTO>();
		}

		public UserTransactionsViewModel() : this(0)
		{ }

		public Guid UserId { get; set; }

		public IEnumerable<TransactionTableDTO> Transactions { get; set; }

		public IEnumerable<AccountDropdownDTO> OwnerAccounts { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> OwnerAccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> OwnerCurrencies { get; set; }

		public IEnumerable<CategoryDropdownDTO> OwnerCategories { get; set; }

		public RoutingModel Routing { get; private set; }

		public PaginationModel Pagination { get; private set; }
	}
}
