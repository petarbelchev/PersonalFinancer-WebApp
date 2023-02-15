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

		public async Task<IActionResult> Index()
		{
			DashboardViewModel dashboard = await accountService.DashboardViewModel(User.Id());

			return View(dashboard);
		}

		public IActionResult Privacy() => View();

		public IActionResult Error() => View();
	}
}