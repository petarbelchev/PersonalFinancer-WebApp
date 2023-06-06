using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Web.Models.Account;
using static PersonalFinancer.Web.Infrastructure.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class AccountsController : Web.Controllers.AccountsController
	{
		public AccountsController(
			IAccountsService accountService,
			IUsersService usersService,
			IMapper mapper)
			: base(accountService, mapper, usersService)
		{ }

		public async Task<IActionResult> Index(int page = 1)
		{
			UsersAccountCardsServiceModel usersAccountCardsData =
				await accountService.GetAccountCardsData(page);

			var viewModel = new UsersAccountCardsViewModel
			{
				Accounts = usersAccountCardsData.Accounts
			};
			viewModel.Pagination.Page = page;
			viewModel.Pagination.TotalElements = usersAccountCardsData.TotalUsersAccountsCount;

			return View(viewModel);
		}
	}
}
