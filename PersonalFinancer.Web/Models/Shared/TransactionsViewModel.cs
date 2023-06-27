namespace PersonalFinancer.Web.Models.Shared
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
    using static PersonalFinancer.Services.Constants;

    public class TransactionsViewModel
    {
        public TransactionsViewModel(
            TransactionsDTO transactionsDTO,
            int page,
            string transactionDetailsUrl)
        {
            this.Transactions = transactionsDTO.Transactions;
            
            this.Pagination = new PaginationModel(
                PaginationConstants.TransactionsName,
                PaginationConstants.TransactionsPerPage,
                transactionsDTO.TotalTransactionsCount, 
                page);

            this.TransactionDetailsUrl = transactionDetailsUrl;
        }

        public IEnumerable<TransactionTableDTO> Transactions { get; private set; }

        public PaginationModel Pagination { get; private set; }

        public string TransactionDetailsUrl { get; private set; }
    }
}
