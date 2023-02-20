namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Services.Account;
	using PersonalFinancer.Services.Account.Models;

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
			var model = new DashboardServiceModel()
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
			await accountService.DashboardViewModel(User.Id(), model);

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			return View(model);
		}

		public IActionResult Privacy() => View();

		public IActionResult Error() => View();
	}
}