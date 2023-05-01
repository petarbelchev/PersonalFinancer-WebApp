namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Data.Models;

    using Services.ApiService;
    using Services.ApiService.Models;
	using Services.Shared.Models;

    using Web.Infrastructure;

	[Authorize]
	[Route("api/categories")]
	[ApiController]
	public class CategoriesApiController : BaseApiController
	{
		private readonly IApiService<Category> apiService;

		public CategoriesApiController(IApiService<Category> apiService)
			=> this.apiService = apiService;

		[HttpPost]
		public async Task<ActionResult> CreateCategory(CategoryInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(GetErrors(ModelState.Values));

			try
			{
				ApiOutputServiceModel model =
					await apiService.CreateEntity(inputModel);

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
				await apiService.DeleteEntity(id, User.Id(), User.IsAdmin());

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
