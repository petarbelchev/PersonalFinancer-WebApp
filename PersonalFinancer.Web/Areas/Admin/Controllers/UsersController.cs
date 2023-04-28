namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	
	using Services.User;
	using Services.User.Models;
	
	using Web.Models.User;

	using static Data.Constants.RoleConstants;

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
			=> View(await userService.UserDetails(id));
	}
}
