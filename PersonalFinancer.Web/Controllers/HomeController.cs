namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Services.Accounts;
	using Services.Accounts.Models;

	/// <summary>
	/// Home Controller takes care of everything related to Dashboard page.
	/// </summary>
	public class HomeController : Controller
	{
		private readonly IAccountService accountService;

		public HomeController(IAccountService accountService)
		{
			this.accountService = accountService;
		}

		/// <summary>
		/// Returns Dashboard Model for Dashboard home page.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			DashboardServiceModel model = new DashboardServiceModel()
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			await accountService.DashboardViewModel(User.Id(), model);

			return View(model);
		}

		/// <summary>
		/// Handle with Dashboard Model for specific period and returns it for render.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Index(DashboardServiceModel model)
		{
			try
			{
				await accountService.DashboardViewModel(User.Id(), model);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(model.EndDate), ex.Message);
			}

			return View(model);
		}

		public IActionResult Privacy() => View();

		public IActionResult Error() => View();
	}
}