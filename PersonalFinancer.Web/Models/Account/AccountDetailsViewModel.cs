namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class AccountDetailsViewModel : AccountDetailsInputModel
	{
		public AccountDetailsViewModel(int totalTransactionsCount)
		{
			this.Pagination = new PaginationModel(
				TransactionsName, TransactionsPerPage, totalTransactionsCount);

			this.Transactions = new List<TransactionTableDTO>();
		}

		public AccountDetailsViewModel() : this(0)
		{ }

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;

		public string AccountTypeName { get; set; } = null!;

		public Guid OwnerId { get; set; }

		public IEnumerable<TransactionTableDTO> Transactions { get; set; }

		public PaginationModel Pagination { get; private set; }

		public string? ReturnUrl { get; set; }
    }
}
