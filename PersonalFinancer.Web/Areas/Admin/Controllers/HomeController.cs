namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Web.Areas.Admin.Models.Home;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using static PersonalFinancer.Data.Constants;
    using static PersonalFinancer.Web.Infrastructure.Constants;

    [Area("Admin")]
    [Authorize(Roles = RoleConstants.AdminRoleName)]
    public class HomeController : Controller
    {
        private readonly IUsersService userService;

        public HomeController(IUsersService userService)
            => this.userService = userService;

        public async Task<IActionResult> Index()
        {
            return this.View(new AdminDashboardViewModel
            {
                RegisteredUsers = await this.userService.UsersCount(),
                CreatedAccounts = await this.userService.GetUsersAccountsCount(),
                AdminFullName = await this.userService.UserFullName(this.User.Id()),
                AccountsCashFlowEndpoint = HostConstants.ApiAccountsCashFlowUrl
            });
        }
    }
}