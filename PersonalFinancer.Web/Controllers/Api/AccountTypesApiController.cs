using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.ModelsState;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;
using System.Text;

namespace PersonalFinancer.Web.Controllers.Api
{
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
		public async Task<ActionResult<AccountTypeViewModel>> Create(AccountTypeInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(modelStateService.GetErrors(ModelState.Values));

			try
			{
				AccountTypeViewModel viewModel =
					await accountTypeService.CreateAccountType(inputModel);

				return Created(string.Empty, viewModel);
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
