namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Services.Accounts.Models;
	using Services.User;

	/// <summary>
	/// Home Controller takes care of everything related to Dashboard page.
	/// </summary>
	public class HomeController : Controller
	{
		private readonly IUserService userService;

		public HomeController(IUserService userService)
		{
			this.userService = userService;
		}

		/// <summary>
		/// Returns Dashboard Model for Dashboard home page.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (User.IsAdmin())
			{
				return LocalRedirect("/Admin");
			}

			if (User.Identity?.IsAuthenticated ?? false)
			{
				DashboardServiceModel model = new DashboardServiceModel()
				{
					StartDate = DateTime.UtcNow.AddMonths(-1),
					EndDate = DateTime.UtcNow
				};

				await userService.GetUserDashboard(User.Id(), model);
				
				return View(model);
			}

			return View();
		}

		/// <summary>
		/// Handle with Dashboard Model for specific period and returns it for render.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Index(DashboardServiceModel model)
		{
			try
			{
				await userService.GetUserDashboard(User.Id(), model);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(model.EndDate), ex.Message);
			}

			return View(model);
		}

		public IActionResult Error() => View();

		public IActionResult AccessDenied() => View();
	}
}