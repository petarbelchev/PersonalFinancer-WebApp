namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Data.Models;

	using Services.Shared;
	using Services.Shared.Models;

	using Web.Infrastructure;

	[Authorize]
	[Route("api/currencies")]
	[ApiController]
	public class CurrenciesApiController : BaseApiController
	{
		private readonly ApiService<Currency> apiService;

		public CurrenciesApiController(ApiService<Currency> apiService)
			=> this.apiService = apiService;

		[HttpPost]
		public async Task<ActionResult> Create(ApiInputServiceModel inputModel)
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
		public async Task<ActionResult> Delete(string id)
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