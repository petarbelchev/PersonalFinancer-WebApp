using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class HomeController : Controller
	{
		private readonly IUserService userService;

		public HomeController(IUserService userService)
			=> this.userService = userService;

		public async Task<IActionResult> Index()
		{
			return View(new AdminDashboardViewModel
			{
				RegisteredUsers = userService.UsersCount(),
				CreatedAccounts = userService.GetUsersAccountsCount(),
				AdminFullName = await userService.FullName(User.Id())
			});
		}
	}
}