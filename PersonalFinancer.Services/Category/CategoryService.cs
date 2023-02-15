namespace PersonalFinancer.Services.Category
{
	using Microsoft.EntityFrameworkCore;

	using Models;
	using Data;
	using static Data.DataConstants.Category;

	public class CategoryService : ICategoryService
	{
		private readonly PersonalFinancerDbContext data;

		public CategoryService(PersonalFinancerDbContext context)
		{
			this.data = context;
		}

		/// <summary>
		/// Returns collection of User's categories with Id and Name.
		/// </summary>
		public async Task<IEnumerable<CategoryViewModel>> UserCategories(string userId)
		{
			return await data.Categories
				.Where(c =>	
					c.Name != CategoryInitialBalanceName && 
					(c.UserId == null || c.UserId == userId))
				.Select(c => new CategoryViewModel()
				{
					Id = c.Id,
					Name = c.Name
				})
				.ToArrayAsync();
		}

		//public async Task<Guid> CategoryId(string categoryName)
		//{
		//	var category = await data.Categories.FirstAsync(c => c.Name == categoryName);

		//	return category.Id;
		//}

		/// <summary>
		/// Returns Category with props: Id and Name, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<CategoryViewModel> CategoryById(Guid categoryId)
		{
			return await data.Categories
				.Where(c => c.Id == categoryId)
				.Select(c => new CategoryViewModel
				{
					Id = c.Id,
					Name = c.Name
				})
				.FirstAsync();
		}

		/// <summary>
		/// Checks is the given Category is an initial balance.
		/// </summary>
		public async Task<bool> IsInitialBalance(Guid categoryId)
		{
			var category = await data.Categories.FindAsync(categoryId);

			if (category == null)
			{
				return false;
			}

			return category.Name == CategoryInitialBalanceName;
		}
	}
}
