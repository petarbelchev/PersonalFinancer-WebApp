using PersonalFinancer.Services.Categories.Models;

namespace PersonalFinancer.Services.Categories
{
	public interface ICategoryService
	{
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist
		/// and ArgumentException when User is not owner or Administrator.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		Task DeleteCategory(string categoryId, string? ownerId = null);
		
		/// <summary>
		/// Throws InvalidOperationException when Category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<CategoryViewModel> GetCategoryViewModel(string categoryId, string ownerId);

		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<string> GetCategoryIdByName(string categoryName);
		
		/// <summary>
		/// Throws ArgumentException if try to create Category with existing name.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		Task<CategoryViewModel> CreateCategory(CategoryInputModel model);
		
		/// <summary>
		/// Throws InvalidOperationException if category does not exist.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<bool> IsInitialBalance(string categoryId);

		Task<IEnumerable<CategoryViewModel>> GetUserCategories(string userId);
	}
}
