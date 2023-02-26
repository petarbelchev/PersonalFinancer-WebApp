namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Services.Category.Models;
	using Services.Category;
	using static Data.DataConstants.Category;

	[Route("api/categories")]
	[ApiController]
	public class CategoryApiController : ControllerBase
	{
		private readonly ICategoryService categoryService;

		public CategoryApiController(ICategoryService categoryService)
		{
			this.categoryService = categoryService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryViewModel>> GetCategory(Guid id)
		{
			CategoryViewModel? category = await categoryService.CategoryById(id);

			if (category == null)
			{
				return NotFound();
			}

			return category;
		}

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryViewModel category)
		{
			if (category.Name.Length < CategoryNameMinLength || category.Name.Length > CategoryNameMaxLength)
			{
				return BadRequest($"Category name must be between {CategoryNameMinLength} and {CategoryNameMaxLength} characters long.");
			}

			try
			{
				CategoryViewModel newCategory = await categoryService
					.CreateCategory(User.Id(), category.Name);

				return Created(string.Empty, newCategory);
			}
			catch (InvalidOperationException)
			{
				return BadRequest("Category with the same name exist. Try another one!");
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCategory(Guid id)
		{
			try
			{
				await categoryService.DeleteCategory(id, User.Id());

				return NoContent();
			}
			catch (ArgumentNullException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
