namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Services.Constants;

	public class AccountDetailsViewModel
	{
        public AccountDetailsViewModel(int totalTransactionsCount)
        {
			this.Pagination = new PaginationModel(
				PaginationConstants.TransactionsName,
				PaginationConstants.TransactionsPerPage,
				totalTransactionsCount);

			this.Routing = new RoutingModel
			{
				Controller = "Accounts",
				Action = "AccountDetails"
			};

			this.Transactions = new List<TransactionTableDTO>();
		}

        public AccountDetailsViewModel() : this(0)
        {
		}

        public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;

        public string AccountTypeName { get; set; } = null!;

        public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public Guid OwnerId { get; set; }

		public IEnumerable<TransactionTableDTO> Transactions { get; set; }

		public RoutingModel Routing { get; private set; }

		public PaginationModel Pagination { get; private set; }
	}
}
