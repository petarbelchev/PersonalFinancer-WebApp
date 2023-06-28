namespace PersonalFinancer.Web.Models.Api
{
	using PersonalFinancer.Web.Models.Transaction;
	using System.ComponentModel.DataAnnotations;

	public class UserTransactionsApiInputModel : UserTransactionsInputModel
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public int Page { get; set; }
    }
}
