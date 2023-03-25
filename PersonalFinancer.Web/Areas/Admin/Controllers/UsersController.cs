using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.User;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class UsersController : Controller
	{
		private readonly IUsersService userService;

		public UsersController(IUsersService userService)
			=> this.userService = userService;

		public async Task<IActionResult> Index(int page = 1)
			=> View(await userService.GetAllUsers(page));

		public async Task<IActionResult> Details(string id)
			=> View(await userService.UserDetails(id));
	}
}
