namespace PersonalFinancer.Services.Category
{
	using Models;

	public interface ICategoryService
	{
		/// <summary>
		/// Returns collection of User's categories with Id and Name.
		/// </summary>
		Task<IEnumerable<CategoryViewModel>> UserCategories(string userId);

		//Task<Guid> CategoryId(string categoryName);

		/// <summary>
		/// Returns Category with props: Id and Name, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		Task<CategoryViewModel> CategoryById(Guid categoryId);

		/// <summary>
		/// Checks is the given Category is an initial balance.
		/// </summary>
		Task<bool> IsInitialBalance(Guid categoryId);
	}
}
