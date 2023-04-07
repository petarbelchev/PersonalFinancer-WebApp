using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Web.Models.Accounts
{
	public class UserTransactionsApiInputModel
	{
		[Required]
		public string Id { get; set; } = null!;

		[Required]
		public string StartDate { get; set; } = null!;

		[Required]
		public string EndDate { get; set; } = null!;

		[Required]
		public int Page { get; set; }
	}
}
