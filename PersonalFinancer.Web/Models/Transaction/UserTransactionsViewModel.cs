namespace PersonalFinancer.Web.Models.Transaction
{
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Services.Infrastructure.Constants;
    using static PersonalFinancer.Web.Infrastructure.Constants;

    public class UserTransactionsViewModel : DateFilterModel
    {
        public Guid Id { get; set; }

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
