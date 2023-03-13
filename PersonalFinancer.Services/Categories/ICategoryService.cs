using PersonalFinancer.Services.Categories.Models;

namespace PersonalFinancer.Services.Categories
{
	public interface ICategoryService
	{
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist or User is not owner.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCategory(Guid categoryId, string userId);
		
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CategoryViewModel> GetCategoryViewModel(Guid categoryId);

		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<Guid> GetCategoryIdByName(string categoryName);
		
		/// <summary>
		/// Throws ArgumentException if try to create Category with existing or invalid name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task CreateCategory(string userId, CategoryViewModel model);
		
		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsInitialBalance(Guid categoryId);

		Task<IEnumerable<CategoryViewModel>> GetUserCategories(string userId);
	}
}
