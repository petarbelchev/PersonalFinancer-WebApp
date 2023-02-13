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
			=> this.data = context;

		public async Task<IEnumerable<CategoryViewModel>> All()
		{
			return await data.Categories
				.Where(c => c.Name != CategoryInitialBalanceName)
				.Select(c => new CategoryViewModel()
				{
					Id = c.Id,
					Name = c.Name
				})
				.ToArrayAsync();
		}

		public async Task<Guid> CategoryIdByName(string name)
		{
			var category = await data.Categories.FirstAsync(c => c.Name == name);

			return category.Id;
		}

		public async Task<CategoryViewModel> CategoryById(Guid id)
		{
			return await data.Categories
				.Where(c => c.Id == id)
				.Select(c => new CategoryViewModel
				{
					Id = c.Id,
					Name = c.Name
				})
				.FirstAsync();
		}

		public async Task<bool> IsInitialBalance(Guid id)
		{
			var category = await data.Categories.FirstAsync(c => c.Id == id);

			return category.Name == CategoryInitialBalanceName;
		}
	}
}
