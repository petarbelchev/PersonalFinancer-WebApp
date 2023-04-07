using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;

using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Models.Home;
using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IUsersService userService;
		private readonly IMapper mapper;

		public HomeController(
			IUsersService userService,
			IMapper mapper)
		{
			this.userService = userService;
			this.mapper = mapper;
		}

		public async Task<IActionResult> Index()
		{
			if (User.IsAdmin())
				return LocalRedirect("/Admin");

			if (User.Identity?.IsAuthenticated ?? false)
			{
				UserDashboardDTO userData = await userService.GetUserDashboardData(
					User.Id(), DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

				var viewModel = mapper.Map<UserDashboardViewModel>(userData);

				return View(viewModel);
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(DateFilterInputModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				IEnumerable<AccountCardDTO> userAccountsData = 
					await userService.GetUserAccounts(User.Id());

				return View(new UserDashboardViewModel
				{
					StartDate = inputModel.StartDate,
					EndDate = inputModel.EndDate,
					Accounts = userAccountsData
						.Select(a => mapper.Map<AccountCardViewModel>(a))
				});
			}

			UserDashboardDTO userData = await userService
				.GetUserDashboardData(User.Id(),
					inputModel.StartDate,
					inputModel.EndDate);

			var viewModel = mapper.Map<UserDashboardViewModel>(userData);

			return View(viewModel);
		}

		public IActionResult Error(int statusCode)
		{
			if (statusCode == 400)
				ViewBag.ImgUrl = "/400-Bad-Request-Error.webp";
			else
				ViewBag.ImgUrl = "/internal-server-error-status-code-500-.webp";

			return View();
		}

		public IActionResult AccessDenied() => View();
	}
}