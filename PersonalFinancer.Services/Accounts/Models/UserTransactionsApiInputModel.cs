namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Services.Accounts.Models;
	using System.ComponentModel.DataAnnotations;

	public class UserTransactionsApiInputModel : UserTransactionsInputModel
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public int Page { get; set; }
    }
}
