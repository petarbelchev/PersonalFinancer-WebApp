namespace PersonalFinancer.Web.Models.Shared
{
	public class CurrencyCashFlowViewModel
    {
        public string Name { get; set; } = null!;

        public decimal Incomes { get; set; }

        public decimal Expenses { get; set; }
    }
}
