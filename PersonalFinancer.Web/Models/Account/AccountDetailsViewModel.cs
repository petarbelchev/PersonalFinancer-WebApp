namespace PersonalFinancer.Web.Models.Account
{
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Services.Constants;
    using static PersonalFinancer.Web.Constants;

    public class AccountDetailsViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;

        public string AccountTypeName { get; set; } = null!;

        public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public Guid OwnerId { get; set; }

		public IEnumerable<TransactionTableServiceModel> Transactions { get; set; }
			= new List<TransactionTableServiceModel>();

		public string ApiTransactionsEndpoint { get; set; }
			= HostConstants.ApiAccountTransactionsUrl;

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Controller = "Accounts",
			Action = "AccountDetails"
		};

		public PaginationModel Pagination { get; set; } = new PaginationModel
		{
			ElementsPerPage = PaginationConstants.TransactionsPerPage,
			ElementsName = PaginationConstants.TransactionsName
		};
	}
}
