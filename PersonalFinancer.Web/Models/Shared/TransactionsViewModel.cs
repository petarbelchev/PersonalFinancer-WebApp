using PersonalFinancer.Services.Shared.Models;
using static PersonalFinancer.Services.Infrastructure.Constants;

namespace PersonalFinancer.Web.Models.Shared
{
	public class TransactionsViewModel
    {
        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; } = null!;

        public PaginationModel Pagination { get; set; } = new PaginationModel
        {
            ElementsName = PaginationConstants.TransactionsName,
            ElementsPerPage = PaginationConstants.TransactionsPerPage
        };

        public string TransactionDetailsUrl { get; set; } = null!;
    }
}
