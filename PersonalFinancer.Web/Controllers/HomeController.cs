using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure.Extensions;
using PersonalFinancer.Web.Models.Home;
using PersonalFinancer.Web.Models.Shared;
using static PersonalFinancer.Data.Constants.HostConstants;

namespace PersonalFinancer.Web.Controllers
{
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
				DateTime startDate = DateTime.Now.AddMonths(-1);
				DateTime endDate = DateTime.Now;

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
				StartDate = inputModel.StartDate.ToUniversalTime(),
				EndDate = inputModel.EndDate.ToUniversalTime()
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
				ViewBag.ImgUrl = BadRequestImgUrl;
			else if (statusCode == 404)
				ViewBag.ImgUrl = NotFoundImgUrl;
			else
				ViewBag.ImgUrl = InternalServerErrorImgUrl;

			return View();
		}

		public IActionResult AccessDenied() => View();
	}
}