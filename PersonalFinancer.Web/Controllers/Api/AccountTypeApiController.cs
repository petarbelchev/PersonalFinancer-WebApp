using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
    [Authorize]
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypeApiController : ControllerBase
	{
		private readonly IAccountService accountService;

		public AccountTypeApiController(IAccountService accountService)
			=> this.accountService = accountService;

		[HttpPost]
		public async Task<ActionResult<AccountTypeViewModel>> Create(AccountTypeInputModel inputModel)
		{
			try
			{
				AccountTypeViewModel viewModel = 
					await accountService.CreateAccountType(inputModel);

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
					await accountService.DeleteAccountType(id);
				else
					await accountService.DeleteAccountType(id, User.Id());

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
