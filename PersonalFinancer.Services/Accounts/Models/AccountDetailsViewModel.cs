using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsViewModel
	{
		public Guid Id { get; set; }

		public string? Name { get; set; }

		public decimal Balance { get; set; }

		public string? CurrencyName { get; set; }

		public IEnumerable<AccountDetailsTransactionViewModel> Transactions { get; set; }
			= new List<AccountDetailsTransactionViewModel>();
		
		[Required(ErrorMessage = "Start Date is required.")]
		[DataType(DataType.DateTime)]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "End Date is required.")]
		[DataType(DataType.DateTime)]
		[Display(Name = "End Date")]
		public DateTime EndDate { get; set; }

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
