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
			=> this.accountService = accountService;

		public async Task<IActionResult> Index()
		{
			string userId = User.Id();

			var dashboardModel = new DashboardViewModel
			{
				LastTransactions = await accountService.LastFiveTransactions(userId),
				Accounts = await accountService.AllAccountsWithData(userId)
			};

			return View(dashboardModel);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Error()
		{
			throw new NotImplementedException();
		}
	}
}