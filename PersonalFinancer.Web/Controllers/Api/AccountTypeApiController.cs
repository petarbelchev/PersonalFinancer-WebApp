using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize]
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypeApiController : ControllerBase
	{
		private readonly IAccountTypeService accountTypeService;

		public AccountTypeApiController(IAccountTypeService accountTypeService)
			=> this.accountTypeService = accountTypeService;

		[HttpPost]
		public async Task<ActionResult<AccountTypeViewModel>> Create(AccountTypeInputModel inputModel)
		{
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
