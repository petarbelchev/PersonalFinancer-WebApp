using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IUsersService userService;

		public HomeController(IUsersService userService)
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
		public async Task<IActionResult> Index(DateFilterModel inputModel)
		{
			var viewModel = new UserDashboardViewModel
			{
				StartDate = inputModel.StartDate,
				EndDate = inputModel.EndDate
			};

			if (!ModelState.IsValid)
			{
				viewModel.Accounts = await userService.GetUserAccounts(User.Id());
				return View(viewModel);
			}

			await userService.SetUserDashboard(User.Id(), viewModel);

			return View(viewModel);
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