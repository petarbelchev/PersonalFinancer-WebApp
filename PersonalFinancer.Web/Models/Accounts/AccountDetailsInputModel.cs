namespace PersonalFinancer.Web.Models.Accounts
{
	using System.ComponentModel.DataAnnotations;

	using Web.Models.Shared;

	public class AccountDetailsInputModel : DateFilterInputModel
	{
		[Required]
		public string Id { get; set; } = null!;
	}
}
