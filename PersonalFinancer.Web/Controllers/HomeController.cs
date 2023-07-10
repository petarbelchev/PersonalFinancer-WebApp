namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Home;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.UrlPathConstants;

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

            if (this.User.Identity?.IsAuthenticated ?? false)
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
        [HttpPost]
        [NotRequireHtmlEncoding]
        public async Task<IActionResult> Index(DateFilterModel inputModel)
        {
            UserDashboardViewModel viewModel;

			if (!this.ModelState.IsValid)
            {
                viewModel = this.mapper.Map<UserDashboardViewModel>(inputModel);
                viewModel.Accounts = await this.usersService.GetUserAccountsCardsAsync(this.User.IdToGuid());

                return this.View(viewModel);
            }

            UserDashboardDTO userDashboardData = await this.usersService.GetUserDashboardDataAsync(
                this.User.IdToGuid(), 
                inputModel.FromLocalTime ?? throw new InvalidOperationException(
                    string.Format(ExceptionMessages.NotNullableProperty, inputModel.FromLocalTime)), 
                inputModel.ToLocalTime ?? throw new InvalidOperationException(
                    string.Format(ExceptionMessages.NotNullableProperty, inputModel.ToLocalTime)));

			viewModel = this.mapper.Map<UserDashboardViewModel>(userDashboardData);

            return this.View(viewModel);
        }

        public IActionResult Error(int statusCode)
        {
            if (statusCode == 400)
                this.ViewBag.ImgUrl = BadRequestImgPath;
            else if (statusCode == 404)
                this.ViewBag.ImgUrl = NotFoundImgPath;
            else
                this.ViewBag.ImgUrl = InternalServerErrorImgPath;

            return this.View();
        }
    }
}