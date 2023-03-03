namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Infrastructure;
	using Models.Admin;
	using Services.Accounts;
	using Services.Transactions;
	using Services.User;
	using static Data.Constants.RoleConstants;

	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class HomeController : Controller
	{
		private readonly IUserService userService;
		private readonly IAccountService accountService;
		private readonly ITransactionsService transactionsService;

		public HomeController(
			IUserService userService,
			IAccountService accountService,
			ITransactionsService transactionsService)
		{
			this.userService = userService;
			this.accountService = accountService;
			this.transactionsService = transactionsService;
		}

		public async Task<IActionResult> Index()
		{
			var statistics = new StatisticsViewModel
			{
				RegisteredUsers = userService.UsersCount(),
				CreatedAccounts = accountService.AccountsCount(),
				TotalCashFlow = await accountService.GetAllAccountsCashFlow()
			};

			ViewBag.AdminFullName = await userService.FullName(User.Id());

			return View(statistics);
		}
	}
}