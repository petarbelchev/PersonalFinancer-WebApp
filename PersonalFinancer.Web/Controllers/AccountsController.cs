using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize(Roles = UserRoleName)]
	public class AccountsController : Controller
	{
		private readonly IAccountsService accountService;

		public AccountsController(IAccountsService accountService)
			=> this.accountService = accountService;

		public async Task<IActionResult> Create()
			=> View(await accountService.GetEmptyAccountFormModel(this.User.Id()));

		[HttpPost]
		public async Task<IActionResult> Create(AccountFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				await accountService.PrepareAccountFormModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				string newAccountId = await accountService.CreateAccount(inputModel);

				TempData["successMsg"] = "You create a new account successfully!";

				return RedirectToAction(nameof(AccountDetails), new { id = newAccountId });
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(nameof(inputModel.Name), "You already have Account with that name.");

				await accountService.PrepareAccountFormModelForReturn(inputModel);

				return View(inputModel);
			}
		}

		public async Task<IActionResult> AccountDetails(string id)
		{
			var inputModel = new AccountDetailsInputModel
			{
				Id = id,
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			try
			{
				AccountDetailsViewModel viewModel =
					await accountService.GetAccountDetailsViewModel(inputModel, User.Id());

				viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + id;

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> AccountDetails(AccountDetailsInputModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				try
				{
					return View(await accountService
						.PrepareAccountDetailsViewModelForReturn(inputModel));
				}
				catch (Exception)
				{
					return BadRequest();
				}
			}

			try
			{
				AccountDetailsViewModel viewModel =
					await accountService.GetAccountDetailsViewModel(inputModel, User.Id());

				viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + inputModel.Id;

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> Delete(string id)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			try
			{
				DeleteAccountViewModel viewModel =
					await accountService.GetDeleteAccountViewModel(id, User.Id());

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(DeleteAccountInputModel inputModel, string returnUrl)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			if (inputModel.ConfirmButton == "reject")
				return LocalRedirect(returnUrl);

			try
			{
				await accountService.DeleteAccount(
					inputModel.Id,
					inputModel.ShouldDeleteTransactions ?? false,
					User.Id());
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your account was successfully deleted!";

			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> EditAccount(string id)
		{
			try
			{
				AccountFormModel viewModel =
					await accountService.GetAccountFormModel(id, User.Id());

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditAccount(
			string id, AccountFormModel inputModel, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				await accountService.PrepareAccountFormModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				await accountService.EditAccount(id, inputModel);

				TempData["successMsg"] = "Your account was successfully edited!";

				return LocalRedirect(returnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(
					nameof(inputModel.Name),
					$"You already have Account with {inputModel.Name} name.");

				await accountService.PrepareAccountFormModelForReturn(inputModel);

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}