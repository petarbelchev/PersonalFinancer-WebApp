namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountCardDTO : AccountDTO
	{
        public decimal Balance { get; set; }

        public string CurrencyName { get; set; } = null!;
	}
}
