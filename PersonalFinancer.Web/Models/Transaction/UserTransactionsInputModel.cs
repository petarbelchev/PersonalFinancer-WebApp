namespace PersonalFinancer.Web.Models.Transaction
{
	using PersonalFinancer.Web.Models.Shared;

	public class UserTransactionsInputModel : DateFilterModel
	{
        public Guid? AccountId { get; set; }

        public Guid? AccountTypeId { get; set; }

        public Guid? CurrencyId { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
