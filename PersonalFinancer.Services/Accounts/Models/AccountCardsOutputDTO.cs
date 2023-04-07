namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountCardsOutputDTO
	{
		public IEnumerable<AccountCardExtendedDTO> Accounts { get; set; } = null!;

        public int Page { get; set; }

        public int AllAccountsCount { get; set; }
    }
}
