namespace PersonalFinancer.Services.Shared.Models
{
	public class TransactionDetailsDTO : TransactionTableDTO
	{
		public string AccountName { get; init; } = null!;
	}
}
