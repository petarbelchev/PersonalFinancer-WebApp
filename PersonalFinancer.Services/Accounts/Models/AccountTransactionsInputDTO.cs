namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountTransactionsInputDTO : AccountDetailsInputDTO
	{
        public int Page { get; set; }

        public int ElementsPerPage { get; set; }

        public string OwnerId { get; set; } = null!;
	}
}
