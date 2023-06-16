namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Transactions;
	using PersonalFinancer.Services.User;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionsController : Web.Controllers.TransactionsController
	{
		public TransactionsController(
			IAccountsService accountsService,
			IUsersService usersService,
			IMapper mapper,
			ITransactionsInfoService transactionsInfoService)
			: base(accountsService, usersService, transactionsInfoService, mapper)
		{ }
	}
}
