namespace PersonalFinancer.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    public class DeleteAccountInputModel
	{
		[Required]
		public Guid? Id { get; set; }

		[Required]
		public bool? ShouldDeleteTransactions { get; set; }

		[Required]
		public string ConfirmButton { get; set; } = null!;

		public string? ReturnUrl { get; set; }
    }
}
