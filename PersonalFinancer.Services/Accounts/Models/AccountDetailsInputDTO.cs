namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsInputDTO
	{
        public string Id { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
