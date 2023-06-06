using PersonalFinancer.Services.User.Models;

namespace PersonalFinancer.Services.Shared.Models
{
	public class CurrencyCashFlowServiceModel
	{
		public string Name { get; set; } = null!;

		public decimal Incomes { get; set; }

		public decimal Expenses { get; set; }

		public IEnumerable<CategoryExpensesServiceModel> ExpensesByCategories { get; set; } = null!;
	}
}
