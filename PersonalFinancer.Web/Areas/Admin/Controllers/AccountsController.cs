using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class AccountsController : Controller
	{
		private readonly IAccountsService accountService;

		public AccountsController(IAccountsService accountService)
			=> this.accountService = accountService;

		public async Task<IActionResult> Index(int page = 1)
			=> View(await accountService.GetUsersAccountCardsViewModel(page));

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
					await accountService.GetAccountDetailsViewModel(inputModel);

				viewModel.Routing.Area = "Admin";
				viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + id;

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
				catch (InvalidOperationException)
				{
					return BadRequest();
				}
			}

			try
			{
				AccountDetailsViewModel viewModel =
					await accountService.GetAccountDetailsViewModel(inputModel);

				viewModel.Routing.Area = "Admin";
				viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + inputModel.Id;

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
					await accountService.GetDeleteAccountViewModel(id);

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
				string ownerId = await accountService.GetOwnerId(inputModel.Id);

				await accountService.DeleteAccount(
					inputModel.Id,
					inputModel.ShouldDeleteTransactions ?? false);

				TempData["successMsg"] = "You successfully delete user's account!";

				return LocalRedirect("/Admin/Users/Details/" + ownerId);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> EditAccount(string id)
		{
			try
			{
				AccountFormModel viewModel =
					await accountService.GetAccountFormModel(id);

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

				TempData["successMsg"] = "You successfully edited user's account!";

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
