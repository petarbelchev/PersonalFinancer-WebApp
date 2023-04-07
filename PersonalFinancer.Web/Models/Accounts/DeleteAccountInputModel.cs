using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Web.Models.Accounts
{
	public class DeleteAccountInputModel
	{
		[Required]
		public string Id { get; set; } = null!;

		[Required]
		public bool? ShouldDeleteTransactions { get; set; }

		[Required]
        public string ConfirmButton { get; set; } = null!;
    }
}
