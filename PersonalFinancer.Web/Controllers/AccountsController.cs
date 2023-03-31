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
				ModelState.AddModelError(
					nameof(inputModel.Name),
					"You already have Account with that name.");

				await accountService.PrepareAccountFormModelForReturn(inputModel);

				return View(inputModel);
			}
		}

		public async Task<IActionResult> AccountDetails(
			string id, string? startDate, string? endDate, int page = 1)
		{
			DetailsAccountViewModel viewModel;

			try
			{
				if (startDate == null || endDate == null)
				{
					viewModel = await accountService.GetAccountDetailsViewModel(
						id, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, page, User.Id());
				}
				else
				{
					viewModel = await accountService.GetAccountDetailsViewModel(
						id, DateTime.Parse(startDate), DateTime.Parse(endDate), page, User.Id());
				}
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + id;
			ViewBag.ModelId = id;

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> AccountDetails(
			string id, [Bind("StartDate,EndDate")] DetailsAccountViewModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				try
				{
					await accountService.PrepareAccountDetailsViewModelForReturn(id, inputModel);

					return View(inputModel);
				}
				catch (Exception)
				{
					return BadRequest();
				}
			}

			DetailsAccountViewModel viewModel;

			try
			{
				viewModel = await accountService.GetAccountDetailsViewModel(id,
					inputModel.StartDate ?? throw new InvalidOperationException("Start Date cannot be null."),
					inputModel.EndDate ?? throw new InvalidOperationException("End Date cannot be null."));
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			viewModel.Routing.ReturnUrl = "/Accounts/AccountDetails/" + id;
			ViewBag.ModelId = id;

			return View(viewModel);
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