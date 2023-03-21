using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize(Roles = UserRoleName)]
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
		public async Task<ActionResult<AccountTypeViewModel>> Create(AccountTypeInputModel inputModel)
		{
			try
			{
				AccountTypeViewModel viewModel = await accountTypeService
					.CreateAccountType(User.Id(), inputModel.Name.Trim());

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
