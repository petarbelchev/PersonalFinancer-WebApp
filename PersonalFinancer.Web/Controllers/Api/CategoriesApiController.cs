namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Services.Categories;
    using Services.Categories.Models;
    using Services.Shared.Models;

    using Web.Infrastructure;

    [Authorize]
	[Route("api/categories")]
	[ApiController]
	public class CategoriesApiController : BaseApiController
	{
		private readonly ICategoryService categoryService;

		public CategoriesApiController(ICategoryService categoryService)
			=> this.categoryService = categoryService;

		[HttpPost]
		public async Task<ActionResult<CategoryServiceModel>> CreateCategory(CategoryInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(GetErrors(ModelState.Values));

			try
			{
				CategoryServiceModel model =
					await categoryService.CreateCategory(inputModel);

				return Created(string.Empty, model);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCategory(string id)
		{
			try
			{
				if (User.IsAdmin())
					await categoryService.DeleteCategory(id);
				else
					await categoryService.DeleteCategory(id, User.Id());

				return NoContent();
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
