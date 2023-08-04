namespace PersonalFinancer.Web.Models.Shared
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class TransactionsViewModel
	{
		public TransactionsViewModel(
			TransactionsDTO transactionsDTO,
			int page)
		{
			this.Transactions = transactionsDTO.Transactions;

			this.Pagination = new PaginationModel(
				TransactionsName, 
				TransactionsPerPage, 
				transactionsDTO.TotalTransactionsCount, 
				page);
		}

		public IEnumerable<TransactionTableDTO> Transactions { get; private set; }

		public PaginationModel Pagination { get; private set; }
	}
}
