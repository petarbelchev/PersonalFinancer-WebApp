using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class UserTransactionsApiInputModel
	{
		[Required]
		public string Id { get; set; } = null!; // TODO: Think for refactoring. Id is the same like OwnerId

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
