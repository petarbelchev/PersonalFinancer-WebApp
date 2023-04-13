namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	
	using Services.User;
	using Services.User.Models;

	using Web.Infrastructure;
	using Web.Models.Home;
	using Web.Models.Shared;

	public class HomeController : Controller
	{
		private readonly IUsersService userService;

		public HomeController(IUsersService userService)
			=> this.userService = userService;

		public async Task<IActionResult> Index()
		{
			if (User.IsAdmin())
				return LocalRedirect("/Admin");

			if (User.Identity?.IsAuthenticated ?? false)
			{
				DateTime startDate = DateTime.UtcNow.AddMonths(-1);
				DateTime endDate = DateTime.UtcNow;
				UserDashboardServiceModel userDashboardData = await userService
					.GetUserDashboardData(User.Id(), startDate, endDate);

				var viewModel = new UserDashboardViewModel
				{
					StartDate = startDate,
					EndDate = endDate,
					Accounts = userDashboardData.Accounts,
					Transactions = userDashboardData.LastTransactions,
					CurrenciesCashFlow = userDashboardData.CurrenciesCashFlow
				};

				return View(viewModel);
			}

			return View();
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Index(DateFilterModel inputModel)
		{
			var viewModel = new UserDashboardViewModel
			{
				StartDate = inputModel.StartDate,
				EndDate = inputModel.EndDate
			};

			if (!ModelState.IsValid)
			{
				viewModel.Accounts = await userService.GetUserAccounts(User.Id());
				return View(viewModel);
			}

			UserDashboardServiceModel userDashboardData =
				await userService.GetUserDashboardData(User.Id(), viewModel.StartDate, viewModel.EndDate);
			viewModel.Accounts = userDashboardData.Accounts;
			viewModel.Transactions = userDashboardData.LastTransactions;
			viewModel.CurrenciesCashFlow = userDashboardData.CurrenciesCashFlow;

			return View(viewModel);
		}

		public IActionResult Error(int statusCode)
		{
			if (statusCode == 400)
				ViewBag.ImgUrl = "/images/400BadRequest.webp";
			else
				ViewBag.ImgUrl = "/images/500InternalServerError.webp";

			return View();
		}

		public IActionResult AccessDenied() => View();
	}
}