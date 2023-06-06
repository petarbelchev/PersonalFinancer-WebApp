using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.User;
using PersonalFinancer.Web.Areas.Admin.Models.Home;
using PersonalFinancer.Web.Infrastructure.Extensions;
using static PersonalFinancer.Web.Infrastructure.Constants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = RoleConstants.AdminRoleName)]
	public class HomeController : Controller
	{
		private readonly IUsersService userService;

		public HomeController(IUsersService userService)
			=> this.userService = userService;

		public async Task<IActionResult> Index()
		{
			return View(new AdminDashboardViewModel
			{
				RegisteredUsers = await userService.UsersCount(),
				CreatedAccounts = await userService.GetUsersAccountsCount(),
				AdminFullName = await userService.FullName(User.Id()),
				AccountsCashFlowEndpoint = HostConstants.ApiAccountsCashFlowUrl
			});
		}
	}
}