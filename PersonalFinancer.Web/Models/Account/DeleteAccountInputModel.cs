using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Web.Models.Account
{
	public class DeleteAccountInputModel
	{
		[Required]
		public string Id { get; set; } = null!;

		[Required]
		public bool? ShouldDeleteTransactions { get; set; }

		[Required]
		public string ConfirmButton { get; set; } = null!;

		[Required]
		public string ReturnUrl { get; set; } = null!;
	}
}
