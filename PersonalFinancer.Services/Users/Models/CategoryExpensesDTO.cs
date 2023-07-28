namespace PersonalFinancer.Services.Users.Models
{
	public class CategoryExpensesDTO
	{
		public string CategoryName { get; set; } = null!;

		public decimal ExpensesAmount { get; set; }
	}
}
