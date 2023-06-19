namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Web.Areas.Admin.Models.Home;
    using PersonalFinancer.Web.Extensions;
    using static PersonalFinancer.Data.Constants;
    using static PersonalFinancer.Web.Constants;

    [Area("Admin")]
    [Authorize(Roles = RoleConstants.AdminRoleName)]
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
                AccountsCashFlowEndpoint = HostConstants.ApiAccountsCashFlowUrl
            });
        }
    }
}