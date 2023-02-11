namespace PersonalFinancer.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Account;
	using Services.Account.Models;
	using Services.Currency;
	using Infrastructure;

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

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id))
				return Unauthorized();

			try
			{
				var accountDetails = await accountService.GetAccountById(id);
				return View(accountDetails);
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}