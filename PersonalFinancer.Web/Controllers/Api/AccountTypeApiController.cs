namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Mvc;
	
	using Infrastructure;
	using Services.AccountTypes;
	using Services.AccountTypes.Models;
	using static Data.Constants.AccountTypeConstants;
	
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

			if (model.Name.Length < AccountTypeNameMinLength || model.Name.Length > AccountTypeNameMaxLength)
			{
				return BadRequest($"Account Type name must be between {AccountTypeNameMinLength} and {AccountTypeNameMaxLength} characters long.");
			}

			try
			{
				AccountTypeViewModel accountType = await accountTypeService
					.CreateAccountType(User.Id(), model.Name);

				return Created(string.Empty, accountType);
			}
			catch (InvalidOperationException)
			{
				return BadRequest("Account Type with the same name exist. Try another one!");
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
			catch (ArgumentNullException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
