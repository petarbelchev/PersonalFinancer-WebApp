#nullable disable

namespace PersonalFinancer.Services.Shared.Models
{
	using PersonalFinancer.Services.Users.Models;

	public class CurrencyCashFlowWithExpensesByCategoriesDTO : CurrencyCashFlowDTO
	{
		public IEnumerable<CategoryExpensesDTO> ExpensesByCategories { get; set; }
	}
}
