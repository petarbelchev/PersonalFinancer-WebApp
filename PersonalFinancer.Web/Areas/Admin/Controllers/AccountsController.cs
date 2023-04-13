namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;
	using Services.User;

	using Web.Models.Account;

	using static Data.Constants.RoleConstants;

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
				await accountService.GetUsersAccountCardsData(page);

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
