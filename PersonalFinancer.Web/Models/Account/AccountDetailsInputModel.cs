namespace PersonalFinancer.Web.Models.Account
{
	using System.ComponentModel.DataAnnotations;

	using Web.Models.Shared;

	public class AccountDetailsInputModel : DateFilterModel
	{
		[Required]
		public string Id { get; set; } = null!;
	}
}
