﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers.Api
{
	[Authorize(Roles = AdminRoleName)]
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
