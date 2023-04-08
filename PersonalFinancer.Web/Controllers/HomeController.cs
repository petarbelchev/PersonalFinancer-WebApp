namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;

	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts.Models;
	using Services.User;
	using Services.User.Models;

	using Web.Infrastructure;
	using Web.Models.Home;
	using Web.Models.Shared;

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

			UserDashboardDTO userData = await userService.GetUserDashboardData(
				User.Id(), inputModel.StartDate, inputModel.EndDate);

			var viewModel = mapper.Map<UserDashboardViewModel>(userData);

			return View(viewModel);
		}

		public IActionResult Error(int statusCode)
		{
			if (statusCode == 400)
				ViewBag.ImgUrl = "/img/400BadRequestError.webp";
			else
				ViewBag.ImgUrl = "/img/500InternalServerError.webp";

			return View();
		}

		public IActionResult AccessDenied() => View();
	}
}