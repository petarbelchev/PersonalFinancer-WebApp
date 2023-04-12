namespace PersonalFinancer.Web.Models.Account
{
	using System.ComponentModel.DataAnnotations;
	
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
