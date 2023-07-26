namespace PersonalFinancer.Web.Models.Api
{
	public class DeleteTransactionOutputModel
	{
		public string Message { get; set; } = null!;

		public decimal Balance { get; set; }
	}
}
