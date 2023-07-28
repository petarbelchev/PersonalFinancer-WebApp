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
	public class TransactionsController : Web.Controllers.TransactionsController
	{
		public TransactionsController(
			IAccountsUpdateService accountsService,
			IAccountsInfoService accountsInfoService,
			IUsersService usersService,
			IMapper mapper)
			: base(accountsService, accountsInfoService, usersService, mapper)
		{ }
	}
}
