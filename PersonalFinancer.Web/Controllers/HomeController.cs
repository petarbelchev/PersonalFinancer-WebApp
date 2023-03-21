using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IUserService userService;

		public HomeController(IUserService userService)
			=> this.userService = userService;

		public async Task<IActionResult> Index()
		{
			if (User.IsAdmin())
				return LocalRedirect("/Admin");

			if (User.Identity?.IsAuthenticated ?? false)
			{
				var viewModel = new UserDashboardViewModel()
				{
					StartDate = DateTime.UtcNow.AddMonths(-1),
					EndDate = DateTime.UtcNow
				};

				await userService.SetUserDashboard(User.Id(), viewModel);

				return View(viewModel);
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(
			[Bind("StartDate,EndDate")]UserDashboardViewModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				inputModel.Accounts = await userService.GetUserAccounts(User.Id());
				return View(inputModel);
			}

			await userService.SetUserDashboard(User.Id(), inputModel);

			return View(inputModel);
		}

		public IActionResult Error(int statusCode)
		{
			if (statusCode == 400)
				ViewBag.ImgUrl = "/400-Bad-Request-Error.webp";
			else
				ViewBag.ImgUrl = "/internal-server-error-status-code-500-.webp";

			return View();
		}

		public IActionResult AccessDenied() => View();
	}
}