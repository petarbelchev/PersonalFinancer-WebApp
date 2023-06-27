namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Services.Constants;
	using static PersonalFinancer.Web.Constants;

	public class UserTransactionsViewModel : UserTransactionsInputModel
	{
		public UserTransactionsViewModel(int totalTransactionsCount, int page = 1)
		{
			this.Pagination = new PaginationModel(
				PaginationConstants.TransactionsName,
				PaginationConstants.TransactionsPerPage,
				totalTransactionsCount,
				page);

			this.ApiTransactionsEndpoint = UrlPathConstants.ApiTransactionsPath;

			this.Routing = new RoutingModel
			{
				Controller = "Transactions",
				Action = "All"
			};

			this.Transactions = new List<TransactionTableDTO>();
			this.UserAccounts = new List<AccountDropdownDTO>();
			this.UserAccountTypes = new List<AccountTypeDropdownDTO>();
			this.UserCurrencies = new List<CurrencyDropdownDTO>();
			this.UserCategories = new List<CategoryDropdownDTO>();
		}

		public UserTransactionsViewModel() : this(0)
		{ }

		public Guid UserId { get; set; }

		public IEnumerable<TransactionTableDTO> Transactions { get; set; }

		public IEnumerable<AccountDropdownDTO> UserAccounts { get; set; }

		public IEnumerable<AccountTypeDropdownDTO> UserAccountTypes { get; set; }

		public IEnumerable<CurrencyDropdownDTO> UserCurrencies { get; set; }

		public IEnumerable<CategoryDropdownDTO> UserCategories { get; set; }

		public string ApiTransactionsEndpoint { get; private set; }

		public RoutingModel Routing { get; private set; }

		public PaginationModel Pagination { get; private set; }
	}
}
