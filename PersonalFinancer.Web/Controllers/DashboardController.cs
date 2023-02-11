namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Account;
	using Services.Account.Models;
	using Infrastructure;

	[Authorize]
	public class DashboardController : Controller
	{
		private readonly IAccountService accountService;

		public DashboardController(
			IAccountService accountService)
		{
			this.accountService = accountService;
		}

		[HttpGet]
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
	}
}
