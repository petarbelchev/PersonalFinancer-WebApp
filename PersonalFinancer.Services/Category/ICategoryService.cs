namespace PersonalFinancer.Services.Category
{
	using Models;

	public interface ICategoryService
	{
		/// <summary>
		/// Delete Category with given Id or throws exception when Category does not exist or User is not owner.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCategory(Guid categoryId, string userId);

		/// <summary>
		/// Returns Category with props: Id and Name or null.
		/// </summary>
		Task<CategoryViewModel?> CategoryById(Guid categoryId);

		/// <summary>
		/// Return Category Id by given name. If category does not exist throw an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		Task<Guid> CategoryIdByName(string categoryName);
		
		/// <summary>
		/// Creates new Category with given Name. Returns View Model with Id, Name and User Id.
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
