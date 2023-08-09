namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Web.Models.Shared;
	using System.ComponentModel.DataAnnotations;

	public class AccountDetailsInputModel : DateFilterModel
	{
		[Required]
		public Guid? Id { get; set; }

		public string? ReturnUrl { get; set; }
	}
}
