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

		[HttpPost]
		public async Task<ActionResult<CategoryViewModel>> CreateCategory([FromBody] string categoryName)
		{
			if (categoryName.Length < CategoryNameMinLength || categoryName.Length > CategoryNameMaxLength)
			{
				return BadRequest($"Category name must be between {CategoryNameMinLength} and {CategoryNameMaxLength} characters long.");
			}

			try
			{
				CategoryViewModel newCategory = await categoryService
					.CreateCategory(User.Id(), categoryName);

				return Ok(newCategory);
			}
			catch (InvalidOperationException)
			{
				return BadRequest("Category with the same name exist. Try another one!");
			}
		}

		[HttpGet("{id}")]
		[Route("delete")]
		public async Task<ActionResult> DeleteCategory(Guid categoryId)
		{
			bool isDeleted = await categoryService.DeleteCategory(categoryId);

			if (!isDeleted)
			{
				return NotFound();
			}

			return Ok(categoryId);
		}
	}
}
