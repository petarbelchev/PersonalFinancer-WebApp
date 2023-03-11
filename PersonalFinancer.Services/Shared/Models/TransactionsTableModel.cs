using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Transactions.Models;

namespace PersonalFinancer.Services.Shared.Models
{
	public class TransactionsTableModel
	{
		public string Name { get; set; } = null!;

		public IEnumerable<TransactionExtendedViewModel>? UserTransactions { get; set; }

		public IEnumerable<AccountDetailsTransactionViewModel>? AccountTransactions { get; set; }

        public IEnumerable<string> Heads
		{
			get
			{
				if (UserTransactions != null)
				{
					return new string[] { "Date", "Category", "Type", "Amount", "Account", "Refference" };
				}
				
				return new string[] { "Date", "Category", "Type", "Amount", "Refference" };
			}
		}
	}
}
