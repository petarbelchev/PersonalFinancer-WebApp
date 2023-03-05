namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using static Data.Constants.CategoryConstants;
	using Services.Category;
	using Services.Category.Models;

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
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

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
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

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
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

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
