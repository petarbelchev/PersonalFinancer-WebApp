using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.ApiService.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypesApiController : BaseApiController
	{
		private readonly IApiService<AccountType> apiService;

		public AccountTypesApiController(IApiService<AccountType> apiService)
			=> this.apiService = apiService;

		[HttpPost]
		public async Task<ActionResult> Create(AccountTypeInputModel inputModel)
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
