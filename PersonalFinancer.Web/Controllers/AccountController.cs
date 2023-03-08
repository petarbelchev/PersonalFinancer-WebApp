using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.Currencies;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	/// <summary>
	/// Account Controller takes care of everything related to Accounts.
	/// </summary>
	[Authorize]
	public class AccountController : Controller
	{
		private readonly IAccountService accountService;
		private readonly ICurrencyService currencyService;
		private readonly IAccountTypeService accountTypeService;

		public AccountController(
			IAccountService accountService,
			ICurrencyService currencyService,
			IAccountTypeService accountTypeService)
		{
			this.accountService = accountService;
			this.currencyService = currencyService;
			this.accountTypeService = accountTypeService;
		}

		/// <summary>
		/// Returns View with Account Form Model for creating new Account.
		/// </summary>
		[HttpGet]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			string userId = User.Id();

			AccountFormModel formModel = new AccountFormModel
			{
				AccountTypes = await accountTypeService.AccountTypesViewModel(userId),
				Currencies = await currencyService.UserCurrencies(userId)
			};

			return View(formModel);
		}

		/// <summary>
		/// Handle with Account Form Model and creates new Account. 
		/// If account was created successfully, redirect to the new Account Details page.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create(AccountFormModel accountFormModel)
		{
			if (!ModelState.IsValid)
			{
				return View(accountFormModel);
			}

			Guid newAccountId = await accountService.CreateAccount(User.Id(), accountFormModel);

			TempData["successMsg"] = "You create a new account successfully!";

			return RedirectToAction(nameof(Details), new { id = newAccountId });
		}

		/// <summary>
		/// Returns Account Details View Model for Account Details page. 
		/// </summary>
		[HttpGet]
		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Details(Guid id)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id))
			{
				return Unauthorized();
			}

			AccountDetailsViewModel? accountDetails = await accountService.AccountDetailsViewModel(id);

			if (accountDetails == null)
			{
				return BadRequest();
			}

			ViewBag.ReturnUrl = "~/Account/Details/" + id;

			return View(accountDetails);
		}

		/// <summary>
		/// Returns Account View Model for Confirm Delete page.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Delete(Guid id, string? returnUrl)
		{
			if (!await accountService.IsAccountOwner(User.Id(), id) && !User.IsAdmin())
			{
				return Unauthorized();
			}

			DeleteAccountViewModel? accountModel = await accountService.DeleteAccountViewModel(id);

			if (accountModel == null)
			{
				return BadRequest();
			}

			if (returnUrl != null)
			{
				accountModel.ReturnUrl = returnUrl;
			}

			return View(accountModel);
		}

		/// <summary>
		/// Handle with deleting an Account.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountViewModel accountModel, string confirmButton)
		{
			if (confirmButton == "reject")
			{
				return LocalRedirect(accountModel.ReturnUrl);
			}

			if (!await accountService.IsAccountOwner(User.Id(), accountModel.Id) && !User.IsAdmin())
			{
				return Unauthorized();
			}

			await accountService.DeleteAccountById(accountModel.Id, User.Id(), accountModel.ShouldDeleteTransactions);

			if (User.IsAdmin())
			{
				TempData["successMsg"] = "You successfully delete user's account!";

				return LocalRedirect("~/Admin/Users/Details/" + accountModel.OwnerId);
			}

			TempData["successMsg"] = "Your account was successfully deleted!";

			return RedirectToAction("Index", "Home");
		}
	}
}