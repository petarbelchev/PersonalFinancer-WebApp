namespace PersonalFinancer.Services.Accounts.Models
{
	public class CreateTransactionInputDTO : BaseTransactionInputDTO
	{
        public bool IsInitialBalance { get; set; }
	}
}
