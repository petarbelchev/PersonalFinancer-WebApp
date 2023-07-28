namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Models.Home;
	using PersonalFinancer.Web.Models.Shared;
	using System.Diagnostics;

	public class HomeController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

		public HomeController(
            IUsersService usersService,
            IMapper mapper)
		{
			this.usersService = usersService;
            this.mapper = mapper;
		}

		public async Task<IActionResult> Index()
        {
            if (this.User.IsAdmin())
                return this.LocalRedirect("/Admin");

            if (this.User.IsAuthenticated())
            {
                DateTime fromLocalTime = DateTime.Now.AddMonths(-1);
                DateTime toLocalTime = DateTime.Now;

                UserDashboardDTO userDashboardData = await this.usersService
                    .GetUserDashboardDataAsync(this.User.IdToGuid(), fromLocalTime, toLocalTime);

				UserDashboardViewModel viewModel = 
                    this.mapper.Map<UserDashboardViewModel>(userDashboardData);

                return this.View(viewModel);
            }

            return this.View();
        }

        [Authorize]
        public async Task<IActionResult> Filtered(DateFilterModel inputModel)
        {
            UserDashboardViewModel viewModel;

			if (!this.ModelState.IsValid)
            {
                viewModel = this.mapper.Map<UserDashboardViewModel>(inputModel);
                viewModel.Accounts = await this.usersService.GetUserAccountsCardsAsync(this.User.IdToGuid());
            }
            else
			{
				UserDashboardDTO userDashboardData = await this.usersService
					.GetUserDashboardDataAsync(this.User.IdToGuid(), inputModel.FromLocalTime, inputModel.ToLocalTime);

				viewModel = this.mapper.Map<UserDashboardViewModel>(userDashboardData);
			}

            return this.View(viewModel);
        }

        public IActionResult StatusCodePage(int statusCode)
        {
            var viewModel = new StatusCodePageViewModel();

            if (statusCode == 400)
            {
                viewModel.Title = "Bad request";
                viewModel.Message = "Something went wrong. Please try again or contact us.";
            }
            else if (statusCode == 401)
			{
				viewModel.Title = "Access denied";
				viewModel.Message = "You do not have access to this resource.";
			}
            else if (statusCode == 404)
			{
				viewModel.Title = "Not found";
				viewModel.Message = "The page you are looking for does not exist.";
			}

            return this.View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier
            });
        }
    }
}