namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Web.Models.Shared;
	using System.ComponentModel.DataAnnotations;

	public class UserTransactionsInputModel : DateFilterModel
	{
        [Display(Name = "Account")]
        public Guid? AccountId { get; set; }

		[Display(Name = "Account Type")]
		public Guid? AccountTypeId { get; set; }

		[Display(Name = "Currency")]
		public Guid? CurrencyId { get; set; }

		[Display(Name = "Category")]
		public Guid? CategoryId { get; set; }
    }
}
