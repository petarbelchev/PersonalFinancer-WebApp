using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypeApiController : ControllerBase
	{
		private readonly IAccountTypeService accountTypeService;

		public AccountTypeApiController(IAccountTypeService accountTypeService)
		{
			this.accountTypeService = accountTypeService;
		}

		[HttpPost]
		public async Task<IActionResult> Create(AccountTypeViewModel model)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

			try
			{
				await accountTypeService.CreateAccountType(User.Id(), model);

				return Created(string.Empty, model);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(Guid id)
		{
			if (!User.Identity?.IsAuthenticated ?? false)
			{
				return Unauthorized();
			}

			try
			{
				await accountTypeService.DeleteAccountType(id, User.Id());

				return NoContent();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
