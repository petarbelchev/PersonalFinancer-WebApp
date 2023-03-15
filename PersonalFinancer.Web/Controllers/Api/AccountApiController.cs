using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Route("api/accounts")]
	[ApiController]
	public class AccountApiController : ControllerBase
	{
		private IAccountService accountService;

		public AccountApiController(IAccountService accountService)
		{
			this.accountService = accountService;
		}

		public async Task<IActionResult> GetAccountsCashFlow()
		{
			var result = await accountService.GetAllAccountsCashFlow();

			return Ok(result);
		}
	}
}
