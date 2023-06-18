namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Web.Models.Account;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class AccountsController : Web.Controllers.AccountsController
    {
        public AccountsController(
            IAccountsUpdateService accountsUpdateService,
            IAccountsInfoService accountsInfoService,
            IUsersService usersService,
            IMapper mapper)
            : base(accountsUpdateService, accountsInfoService, usersService, mapper)
        { }

        public async Task<IActionResult> Index(int page = 1)
        {
            UsersAccountsCardsServiceModel usersAccountCardsData =
                await this.accountsInfoService.GetAccountsCardsDataAsync(page);

            var viewModel = new UsersAccountCardsViewModel
            {
                Accounts = usersAccountCardsData.Accounts
            };
            viewModel.Pagination.Page = page;
            viewModel.Pagination.TotalElements = usersAccountCardsData.TotalUsersAccountsCount;

            return this.View(viewModel);
        }
    }
}
