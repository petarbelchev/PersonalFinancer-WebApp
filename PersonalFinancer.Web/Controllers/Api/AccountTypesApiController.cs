namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.AccountTypes;
	using Services.AccountTypes.Models;
	using Services.ModelsState;
	using Services.Shared.Models;

	using Web.Infrastructure;

	[Authorize]
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypesApiController : ControllerBase
	{
		private readonly IAccountTypeService accountTypeService;
		private readonly IModelStateService modelStateService;

		public AccountTypesApiController(
			IAccountTypeService accountTypeService,
			IModelStateService modelStateService)
		{
			this.accountTypeService = accountTypeService;
			this.modelStateService = modelStateService;
		}

		[HttpPost]
		public async Task<ActionResult<AccountTypeServiceModel>> Create(AccountTypeInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(modelStateService.GetErrors(ModelState.Values));

			try
			{
				AccountTypeServiceModel model =
					await accountTypeService.CreateAccountType(inputModel);

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
				if (User.IsAdmin())
					await accountTypeService.DeleteAccountType(id);
				else
					await accountTypeService.DeleteAccountType(id, User.Id());

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
