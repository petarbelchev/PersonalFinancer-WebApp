﻿namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Web.Areas.Admin.Models.Home;
	using static PersonalFinancer.Common.Constants.RoleConstants;
	using static PersonalFinancer.Common.Constants.UrlPathConstants;

	[Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class HomeController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IAccountsInfoService accountsInfoService;

		public HomeController(
            IUsersService userService,
            IAccountsInfoService accountsInfoService)
		{
			this.usersService = userService;
            this.accountsInfoService = accountsInfoService;
		}

		public async Task<IActionResult> Index()
        {
            return this.View(new AdminDashboardViewModel
            {
                RegisteredUsers = await this.usersService.UsersCountAsync(),
                CreatedAccounts = await this.accountsInfoService.GetAccountsCountAsync(),
                AdminFullName = await this.usersService.UserFullNameAsync(this.User.IdToGuid()),
                AccountsCashFlowEndpoint = ApiAccountsCashFlowEndpoint
            });
        }
    }
}