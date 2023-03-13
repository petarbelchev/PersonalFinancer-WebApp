using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.User;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.Admin;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
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
				CreatedAccounts = accountService.GetUsersAccountsCount(),
				TotalCashFlow = await accountService.GetAllAccountsCashFlow()
			};

			ViewBag.AdminFullName = await userService.FullName(User.Id());

			return View(statistics);
		}
	}
}