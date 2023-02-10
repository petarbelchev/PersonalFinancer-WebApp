using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Account;
using PersonalFinancer.Services.Account.Models;
using PersonalFinancer.Services.Currency;
using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly IAccountService accountService;
		private readonly ICurrencyService currencyService;

		public AccountController(
			IAccountService accountService,
			ICurrencyService currencyService)
		{
			this.accountService = accountService;
			this.currencyService = currencyService;
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			string userId = User.Id();

			var formModel = new CreateAccountFormModel
			{
				AccountTypes = await accountService.AllAccountTypes(userId),
				Currencies = await currencyService.AllCurrencies(userId)
			};

			return View(formModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateAccountFormModel accountFormModel)
		{
			if (!ModelState.IsValid)
				return View(accountFormModel);

			try
			{
				await accountService.CreateAccount(User.Id(), accountFormModel);
			}
			catch (Exception)
			{
				return BadRequest();
			}

			//TODO: Implemend redirect to new Accound Details Page!
			return RedirectToAction("Index", "Dashboard");
		}
	}
}
