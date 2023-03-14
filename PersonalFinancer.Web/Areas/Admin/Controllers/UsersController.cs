using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.User;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class UsersController : Controller
	{
		private readonly IUserService userService;
		private readonly IAccountService accountService;

		public UsersController(
			IUserService userService,
			IAccountService accountService)
		{
			this.userService = userService;
			this.accountService = accountService;
		}

		public async Task<IActionResult> Index()
			=> View(await userService.GetAllUsers());

		public async Task<IActionResult> Details(string id)
			=> View(await userService.UserDetails(id));
	}
}
