namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Services.Constants;
	using static PersonalFinancer.Web.Constants;

	public class UserTransactionsViewModel : UserTransactionsInputModel
    {
        public Guid Id { get; set; }

        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; }
            = new List<TransactionTableServiceModel>();

        public IEnumerable<AccountServiceModel> Accounts { get; set; }
            = new List<AccountServiceModel>();

        public IEnumerable<AccountTypeServiceModel> AccountTypes { get; set; } 
            = new List<AccountTypeServiceModel>();

        public IEnumerable<CurrencyServiceModel> Currencies { get; set; } 
            = new List<CurrencyServiceModel>();

        public IEnumerable<CategoryServiceModel> Categories { get; set; } 
            = new List<CategoryServiceModel>();

        public string ApiTransactionsEndpoint { get; set; }
            = HostConstants.ApiTransactionsUrl;

        public RoutingModel Routing { get; set; } = new RoutingModel
        {
            Controller = "Transactions",
            Action = "All"
        };

        public PaginationModel Pagination { get; set; } = new PaginationModel
        {
            ElementsPerPage = PaginationConstants.TransactionsPerPage,
            ElementsName = PaginationConstants.TransactionsName
        };
    }
}
