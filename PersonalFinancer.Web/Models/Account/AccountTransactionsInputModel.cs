namespace PersonalFinancer.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    public class AccountTransactionsInputModel
	{
		[Required]
		public Guid? Id { get; set; }

		[Required]
		public string StartDate { get; set; } = null!;

		[Required]
		public string EndDate { get; set; } = null!;

		[Required]
		public int Page { get; set; }

		[Required]
		public Guid? OwnerId { get; set; }
	}
}
