using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Transactions.Models
{
	public class AllTransactionsServiceModel : DateFilterModel
	{
		public IEnumerable<TransactionExtendedViewModel> Transactions { get; set; }
			= new List<TransactionExtendedViewModel>();

		public int TransactionsPerPage { get; set; } = 10;

		public int Page { get; set; } = 1;

		public int TotalTransactions { get; set; }

		public int FirstTransaction 
			=> TransactionsPerPage * (Page - 1) + 1;

		public int LastTransaction
		{
			get
			{
				int result = TransactionsPerPage * Page;

				if (result > TotalTransactions)
				{
					result = TotalTransactions;
				}

				return result;
			}
		}

		public int Pages
		{
			get
			{
				int result = TotalTransactions / TransactionsPerPage;

				if (TotalTransactions % TransactionsPerPage != 0)
				{
					result++;
				}

				return result;
			}
		}
	}
}
