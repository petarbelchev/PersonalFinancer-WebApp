﻿namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.User;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionsController : Web.Controllers.TransactionsController
	{
		public TransactionsController(
			IAccountsService accountsService,
			IUsersService usersService,
			IMapper mapper)
			: base(accountsService, usersService, mapper)
		{ }
	}
}
