namespace PersonalFinancer.Web.Models.Accounts
{
	using System.ComponentModel.DataAnnotations;

	public class EditAccountFormModel : CreateAccountFormModel
	{
		[Required]
		public string Id { get; set; } = null!;

		[Required]
		public string ReturnUrl { get; set; } = null!;
	}
}
