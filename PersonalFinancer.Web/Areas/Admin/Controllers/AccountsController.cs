namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Users;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class AccountsController : Web.Controllers.AccountsController
    {
        public AccountsController(
            IAccountsUpdateService accountsUpdateService,
            IAccountsInfoService accountsInfoService,
            IUsersService usersService,
            IMapper mapper,
			ILogger<Web.Controllers.AccountsController> logger)
            : base(accountsUpdateService, accountsInfoService, usersService, mapper, logger)
        { }

		public IActionResult Index(string? search)
		{
			this.ViewBag.Search = search;

			return this.View();
		}
	}
}
