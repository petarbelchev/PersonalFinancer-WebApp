namespace PersonalFinancer.Services.Account.Models
{
	public class EditTransactionFormModel : TransactionFormModel
	{
		public Guid Id { get; set; }

		public string? ReturnUrl { get; set; }
	}
}
