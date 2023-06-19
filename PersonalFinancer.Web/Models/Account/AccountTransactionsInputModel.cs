namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Web.Models.Shared;
	using System.ComponentModel.DataAnnotations;

    public class AccountTransactionsInputModel : DateFilterModel
	{
		[Required]
		public Guid? Id { get; set; }

		[Required]
		public int Page { get; set; }

		[Required]
		public Guid? OwnerId { get; set; }
	}
}
