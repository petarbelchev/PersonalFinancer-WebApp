namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;
	using PersonalFinancer.Web.Models.Api;
	using System.ComponentModel.DataAnnotations;

	[Route("api/categories")]
	[ApiController]
	public class CategoriesApiController : BaseApiController<Category>
	{
		public CategoriesApiController(
			IApiService<Category> apiService,
			ILogger<BaseApiController<Category>> logger)
			: base(apiService, logger)
		{ }

		[HttpPost]
		[Produces("application/json")]
		[ProducesResponseType(typeof(ApiEntityDTO), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateCategory(CategoryInputModel inputModel)
			=> await this.CreateEntityAsync(inputModel);

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteCategory([Required] Guid id)
			=> await this.DeleteEntityAsync(id);
	}
}
