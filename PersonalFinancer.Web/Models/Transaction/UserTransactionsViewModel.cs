namespace PersonalFinancer.Web.Models.Transaction
{
    using Services.Shared.Models;

    using Web.Models.Shared;

    using static Data.Constants;

    public class UserTransactionsViewModel : DateFilterModel
    {
        public string Id { get; set; } = null!;

        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; }
            = new List<TransactionTableServiceModel>();

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
