namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Services.Account;
	using Services.Account.Models;

	public class HomeController : Controller
	{
		private readonly IAccountService accountService;

		public HomeController(IAccountService accountService)
		{
			this.accountService = accountService;
		}

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