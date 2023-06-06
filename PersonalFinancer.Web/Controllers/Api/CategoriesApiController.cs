using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.ApiService.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure.Extensions;

namespace PersonalFinancer.Web.Controllers.Api
{
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
