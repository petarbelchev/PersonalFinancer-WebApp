namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;
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
