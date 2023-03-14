using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IUserService userService;

		public HomeController(IUserService userService)
		{
			this.userService = userService;
		}

		public async Task<IActionResult> Index()
		{
			if (User.IsAdmin())
			{
				return LocalRedirect("/Admin");
			}

			if (User.Identity?.IsAuthenticated ?? false)
			{
				var model = new HomeIndexViewModel()
				{
					Dates = new DateFilterModel
					{
						StartDate = DateTime.UtcNow.AddMonths(-1),
						EndDate = DateTime.UtcNow
					}
				};

				await userService.GetUserDashboard(User.Id(), model);

				return View(model);
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(DateFilterModel dateModel)
		{
			if (!ModelState.IsValid)
			{
				return View(dateModel);
			}

			var model = new HomeIndexViewModel
			{
				Dates = dateModel
			};

			try
			{
				await userService.GetUserDashboard(User.Id(), model);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(model.Dates.EndDate), ex.Message);
			}

			return View(model);
		}

		public IActionResult Error(int statusCode)
		{
			if (statusCode == 400)
			{
				ViewBag.ImgUrl = "/400-Bad-Request-Error.webp";
			}
			else
			{
				ViewBag.ImgUrl = "/internal-server-error-status-code-500-.webp";
			}

			return View();
		}

		public IActionResult AccessDenied() => View();
	}
}