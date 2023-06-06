using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Web.Models.Account
{
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
