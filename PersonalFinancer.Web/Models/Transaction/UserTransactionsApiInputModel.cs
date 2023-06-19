namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Web.Models.Shared;
	using System.ComponentModel.DataAnnotations;

    public class UserTransactionsApiInputModel : DateFilterModel
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public int Page { get; set; }
    }
}
