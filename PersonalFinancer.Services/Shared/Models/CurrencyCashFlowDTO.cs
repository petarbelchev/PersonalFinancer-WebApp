#nullable disable

namespace PersonalFinancer.Services.Shared.Models
{
	public class CurrencyCashFlowDTO
    {
        public string Name { get; set; }

        public decimal Incomes { get; set; }

        public decimal Expenses { get; set; }
    }
}
