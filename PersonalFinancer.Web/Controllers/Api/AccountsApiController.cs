using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize(Roles = AdminRoleName)]
	[Route("api/accounts")]
	[ApiController]
	public class AccountsApiController : ControllerBase
	{
		private IAccountsService accountService;

		public AccountsApiController(IAccountsService accountService)
			=> this.accountService = accountService;

		public async Task<IActionResult> GetAccountsCashFlow()
			=> Ok(await accountService.GetUsersAccountsCashFlow());
	}
}
