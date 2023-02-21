namespace PersonalFinancer.Services.Category
{
	using Models;

	public interface ICategoryService
	{
		/// <summary>
		/// Delete Category with given Id. Returns True when category was deleted or False when does not.
		/// </summary>
		Task<bool> DeleteCategory(Guid categoryId);

		/// <summary>
		/// Returns Category with props: Id and Name, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		Task<CategoryViewModel> CategoryById(Guid categoryId);
		
		/// <summary>
		/// Creates new Category with given Name. Returns View Model with Id and Name.
		/// If try to create Category with name that other category have, throws exception.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CategoryViewModel> CreateCategory(string userId, string categoryName);

		/// <summary>
		/// Checks is the given Category is an initial balance.
		/// </summary>
		Task<bool> IsInitialBalance(Guid categoryId);

		/// <summary>
		/// Returns collection of User's categories with Id and Name.
		/// </summary>
		Task<IEnumerable<CategoryViewModel>> UserCategories(string userId);
	}
}
