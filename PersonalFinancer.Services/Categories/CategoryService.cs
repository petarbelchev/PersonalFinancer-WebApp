using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Categories.Models;
using static PersonalFinancer.Data.Constants.CategoryConstants;

namespace PersonalFinancer.Services.Categories
{
	public class CategoryService : ICategoryService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly IMemoryCache memoryCache;

		public CategoryService(
			PersonalFinancerDbContext context,
			IMapper mapper,
			IMemoryCache memoryCache)
		{
			this.data = context;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
		}

		/// <summary>
		/// Delete Category with given Id or throws exception when Category does not exist or User is not owner.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task DeleteCategory(Guid categoryId, string userId)
		{
			Category? category = await data.Categories.FindAsync(categoryId);

			if (category == null)
			{
				throw new ArgumentNullException("Category does not exist.");
			}

			if (category.UserId != userId)
			{
				throw new InvalidOperationException("You can't delete someone else category.");
			}

			category.IsDeleted = true;

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);
		}

		/// <summary>
		/// Returns Category with props: Id and Name or null.
		/// </summary>
		public async Task<CategoryViewModel?> CategoryById(Guid categoryId)
		{
			CategoryViewModel? category = await data.Categories
				.Where(c => c.Id == categoryId)
				.Select(c => mapper.Map<CategoryViewModel>(c))
				.FirstOrDefaultAsync();

			return category;
		}

		/// <summary>
		/// Return Category Id by given name. If category does not exist throw an exception.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<Guid> CategoryIdByName(string categoryName)
		{
			Category category = await data.Categories.FirstAsync(c => c.Name == categoryName);

			return category.Id;
		}

		/// <summary>
		/// Creates new Category with given Name. Returns View Model with Id, Name and User Id.
		/// If try to create Category with name that other category have, or name is invalid, throws exception.
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
				category.Name = categoryName.Trim();
			}
			else
			{
				if (categoryName.Length < CategoryNameMinLength || categoryName.Length > CategoryNameMaxLength)
				{
					throw new InvalidOperationException($"Category name must be between {CategoryNameMinLength} and {CategoryNameMaxLength} characters long.");
				}

				category = new Category
				{
					Name = categoryName,
					UserId = userId
				};

				data.Categories.Add(category);
			}

			await data.SaveChangesAsync();

			memoryCache.Remove(CacheKeyValue + userId);

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
			string cacheKey = CacheKeyValue + userId;

			var categories = await memoryCache.GetOrCreateAsync<IEnumerable<CategoryViewModel>>(cacheKey, async cacheEntity =>
			{
				cacheEntity.SetAbsoluteExpiration(TimeSpan.FromDays(3));

				return await data.Categories
					.Where(c => c.UserId == userId && !c.IsDeleted)
					.Select(c => mapper.Map<CategoryViewModel>(c))
					.ToArrayAsync();
			});

			return categories;
		}
	}
}
