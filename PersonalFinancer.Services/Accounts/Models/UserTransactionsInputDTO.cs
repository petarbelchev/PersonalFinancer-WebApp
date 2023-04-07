namespace PersonalFinancer.Services.Accounts.Models
{
	public class UserTransactionsInputDTO
	{
		public string Id { get; set; } = null!;

		public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ElementsPerPage { get; set; }
    }
}
