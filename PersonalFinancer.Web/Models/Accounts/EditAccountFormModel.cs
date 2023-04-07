using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Web.Models.Accounts
{
	public class EditAccountFormModel : CreateAccountFormModel
	{
		[Required]
        public string Id { get; set; } = null!;

		[Required]
        public string ReturnUrl { get; set; } = null!;
    }
}
