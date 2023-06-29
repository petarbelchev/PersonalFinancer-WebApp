namespace PersonalFinancer.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Common.Messages;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Extensions;
    using PersonalFinancer.Web.Models.Home;
    using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Common.Constants.UrlPathConstants;

    public class HomeController : Controller
    {
        private readonly IUsersService userService;
        private readonly IAccountsInfoService accountsInfoService;

		public HomeController(
            IUsersService userService,
            IAccountsInfoService accountsInfoService)
		{
			this.userService = userService;
            this.accountsInfoService = accountsInfoService;
		}

		public async Task<IActionResult> Index()
        {
            if (this.User.IsAdmin())
                return this.LocalRedirect("/Admin");

            if (this.User.Identity?.IsAuthenticated ?? false)
            {
                DateTime startDate = DateTime.Now.AddMonths(-1);
                DateTime endDate = DateTime.Now;

                UserDashboardDTO userDashboardData = await this.userService
                    .GetUserDashboardDataAsync(this.User.IdToGuid(), startDate, endDate);

                var viewModel = new UserDashboardViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Accounts = userDashboardData.Accounts,
                    Transactions = userDashboardData.LastTransactions,
                    CurrenciesCashFlow = userDashboardData.CurrenciesCashFlow
                };

                return this.View(viewModel);
            }

            return this.View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(DateFilterModel inputModel)
        {
            var viewModel = new UserDashboardViewModel
            {
                StartDate = inputModel.StartDate?.ToUniversalTime(),
                EndDate = inputModel.EndDate?.ToUniversalTime()
            };

            if (!this.ModelState.IsValid)
            {
                viewModel.Accounts = await this.accountsInfoService.GetUserAccountsCardsAsync(this.User.IdToGuid());

                return this.View(viewModel);
            }

            UserDashboardDTO userDashboardData =
                await this.userService.GetUserDashboardDataAsync(
                    this.User.IdToGuid(), 
                    viewModel.StartDate ?? throw new InvalidOperationException(
                        string.Format(ExceptionMessages.NotNullableProperty, inputModel.StartDate)), 
                    viewModel.EndDate ?? throw new InvalidOperationException(
                        string.Format(ExceptionMessages.NotNullableProperty, inputModel.EndDate)));
            
            viewModel.Accounts = userDashboardData.Accounts;
            viewModel.Transactions = userDashboardData.LastTransactions;
            viewModel.CurrenciesCashFlow = userDashboardData.CurrenciesCashFlow;

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