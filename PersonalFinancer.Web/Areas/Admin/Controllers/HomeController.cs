using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.User;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.User;
using static PersonalFinancer.Data.Constants;

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
				RegisteredUsers = userService.UsersCount(),
				CreatedAccounts = userService.GetUsersAccountsCount(),
				AdminFullName = await userService.FullName(User.Id()),
				AccountsCashFlowEndpoint = HostConstants.ApiAccountsCashFlowUrl
			});
		}
	}
}