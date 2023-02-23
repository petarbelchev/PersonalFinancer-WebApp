namespace PersonalFinancer.Services.Category
{
	using Microsoft.EntityFrameworkCore;
	using AutoMapper;

	using Models;
	using Data;
	using Data.Models;
	using static Data.DataConstants.Category;

	public class CategoryService : ICategoryService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;

		public CategoryService(
			PersonalFinancerDbContext context,
			IMapper mapper)
		{
			this.data = context;
			this.mapper = mapper;
		}

		/// <summary>
		/// Delete Category with given Id. Returns True when category was deleted or False when does not.
		/// </summary>
		public async Task<bool> DeleteCategory(Guid categoryId)
		{
			Category? category = await data.Categories.FindAsync(categoryId);

			if (category == null)
			{
				return false;
			}

			category.IsDeleted = true;
			await data.SaveChangesAsync();

			return true;
		}

		/// <summary>
		/// Returns Category with props: Id and Name, or throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="OperationCanceledException"></exception>
		public async Task<CategoryViewModel> CategoryById(Guid categoryId)
		{
			CategoryViewModel category = await data.Categories
				.Where(c => c.Id == categoryId)
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.FirstAsync();

			return category;
		}

		/// <summary>
		/// Creates new Category with given Name. Returns View Model with Id and Name.
		/// If try to create Category with name that other category have, throws exception.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CategoryViewModel> CreateCategory(string userId, string categoryName)
		{
			Category? category = await data.Categories
				.FirstOrDefaultAsync(c => c.Name == categoryName && c.UserId == userId);

			if (category != null)
			{
				if (category.IsDeleted == false)
				{
					throw new InvalidOperationException("Category with the same name exist!");
				}

				category.IsDeleted = false;
			}
			else
			{
				category = new Category
				{
					Name = categoryName,
					UserId = userId
				};

				data.Categories.Add(category);
			}

			await data.SaveChangesAsync();

			return mapper.Map<CategoryViewModel>(category);
		}

		/// <summary>
		/// Checks is the given Category is an initial balance.
		/// </summary>
		public async Task<bool> IsInitialBalance(Guid categoryId)
		{
			Category? category = await data.Categories.FindAsync(categoryId);

			if (category == null)
			{
				return false;
			}

			return category.Name == CategoryInitialBalanceName;
		}

		/// <summary>
		/// Returns collection of User's categories with Id and Name.
		/// </summary>
		public async Task<IEnumerable<CategoryViewModel>> UserCategories(string userId)
		{
			IEnumerable<CategoryViewModel> categories = await data.Categories
				.Where(c =>
					c.Name != CategoryInitialBalanceName && !c.IsDeleted &&
					(c.UserId == null || c.UserId == userId))
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.ToArrayAsync();

			return categories;
		}
	}
}
