namespace PersonalFinancer.Services.Accounts.Models
{
	public class TransactionsFilterDTO
	{
        public Guid UserId { get; set; }

        public Guid? AccountId { get; set; }

		public Guid? AccountTypeId { get; set; }

		public Guid? CurrencyId { get; set; }

		public Guid? CategoryId { get; set; }

		public DateTime FromLocalTime { get; set; }

		public DateTime ToLocalTime { get; set; }

		public int Page { get; set; } = 1;
    }
}
