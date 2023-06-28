#nullable disable

namespace PersonalFinancer.Services.Shared.Models
{
	using PersonalFinancer.Services.User.Models;

	public class CurrencyCashFlowWithExpensesByCategoriesDTO : CurrencyCashFlowDTO
	{
		public IEnumerable<CategoryExpensesDTO> ExpensesByCategories { get; set; }
	}
}
