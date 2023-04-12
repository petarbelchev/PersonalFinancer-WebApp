namespace PersonalFinancer.Web.Models.Account
{
	using System.ComponentModel.DataAnnotations;

	public class AccountTransactionsInputModel
	{
		[Required]
		public string Id { get; set; } = null!;

		[Required]
		public string StartDate { get; set; } = null!;

		[Required]
		public string EndDate { get; set; } = null!;

		[Required]
		public int Page { get; set; }

		[Required]
		public string OwnerId { get; set; } = null!;
	}
}
