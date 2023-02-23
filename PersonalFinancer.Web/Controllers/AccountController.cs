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

			var formModel = new AccountFormModel
			{
				AccountTypes = await accountService.AccountTypesViewModel(userId),
				Currencies = await currencyService.UserCurrencies(userId)
			};

			return View(formModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(AccountFormModel accountFormModel)
		{
			if (!ModelState.IsValid)
				return View(accountFormModel);

			await accountService.CreateAccount(User.Id(), accountFormModel);

			TempData["successMsg"] = "You create a new account successfully!";

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id))
				return Unauthorized();

			var accountDetails = await accountService.AccountDetailsViewModel(id);

			return View(accountDetails);
		}

		[HttpGet]
		public async Task<IActionResult> ConfirmDelete(Guid id)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id))
				return Unauthorized();

			AccountDropdownViewModel accountModel = await accountService.AccountDropdownViewModel(id);

			return View(accountModel);
		}

		[HttpGet]
		public async Task<IActionResult> Delete(Guid id, bool shouldDeleteTransactions)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id))
				return Unauthorized();

			await accountService.DeleteAccountById(id, shouldDeleteTransactions);

			TempData["successMsg"] = "Your account was successfully deleted!";

			return RedirectToAction("Index", "Home");
		}
	}
}