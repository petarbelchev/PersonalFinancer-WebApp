namespace PersonalFinancer.Web.Models.Transaction
{
    using System.ComponentModel.DataAnnotations;

    public class UserTransactionsApiInputModel
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string StartDate { get; set; } = null!;

        [Required]
        public string EndDate { get; set; } = null!;

        [Required]
        public int Page { get; set; }
    }
}
