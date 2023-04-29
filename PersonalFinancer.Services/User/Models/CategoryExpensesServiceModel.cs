namespace PersonalFinancer.Services.User.Models
{
	public class CategoryExpensesServiceModel
	{
        public string CategoryName { get; set; } = null!;

        public decimal ExpensesAmount { get; set; }
    }
}
