using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Models.User;
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
		{
			UsersServiceModel usersData = await userService.GetAllUsers(page);
			var viewModel = new UsersViewModel { Users = usersData.Users };
			viewModel.Pagination.TotalElements = usersData.TotalUsersCount;
			viewModel.Pagination.Page = page;

			return View(viewModel);
		}

		public async Task<IActionResult> Details(string id)
		{
			try
			{
				return View(await userService.UserDetails(id));
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
