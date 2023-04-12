namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.User;

	using Web.Areas.Admin.Models.Home;
	using Web.Infrastructure;

	using static Data.Constants;

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