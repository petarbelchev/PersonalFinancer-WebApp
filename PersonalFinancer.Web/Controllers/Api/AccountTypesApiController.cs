namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Services.AccountTypes;
    using Services.AccountTypes.Models;
    using Services.Shared.Models;

    using Web.Infrastructure;

    [Authorize]
	[Route("api/accounttypes")]
	[ApiController]
	public class AccountTypesApiController : BaseApiController
	{
		private readonly IAccountTypeService accountTypeService;

		public AccountTypesApiController(IAccountTypeService accountTypeService)
			=> this.accountTypeService = accountTypeService;

		[HttpPost]
		public async Task<ActionResult<AccountTypeServiceModel>> Create(AccountTypeInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(GetErrors(ModelState.Values));

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
