namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountTransactionsFilterDTO
	{
		public Guid AccountId { get; set; } 
		
		public DateTime StartDate { get; set; } 
		
		public DateTime EndDate { get; set; }

		public int Page { get; set; } = 1;
	}
}
