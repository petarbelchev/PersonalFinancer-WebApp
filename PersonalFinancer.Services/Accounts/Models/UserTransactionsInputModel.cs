namespace PersonalFinancer.Services.Accounts.Models
{
	using PersonalFinancer.Services.Shared.Models;

	public class UserTransactionsInputModel : DateFilterModel
	{
        public string? AccountId { get; set; }

        public string? AccountTypeId { get; set; }

        public string? CurrencyId { get; set; }

        public string? CategoryId { get; set; }
    }
}
