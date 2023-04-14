namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Data.Models;

    using Services.ApiService;
    using Services.ApiService.Models;

    using Web.Infrastructure;

    [Authorize]
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypesApiController : BaseApiController
	{
		private readonly ApiService<AccountType> apiService;

		public AccountTypesApiController(ApiService<AccountType> apiService)
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
			catch (ArgumentException)
			{
				return BadRequest($"Account Type with the name \"{inputModel.Name}\" exist.");
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
