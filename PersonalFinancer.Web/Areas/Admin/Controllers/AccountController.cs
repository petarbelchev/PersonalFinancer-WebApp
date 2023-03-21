﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class AccountController : Controller
	{
		private readonly IAccountService accountService;

		public AccountController(IAccountService accountService)
			=> this.accountService = accountService;

		public async Task<IActionResult> Index(int page = 1)
			=> View(await accountService.GetAllUsersAccountCardsViewModel(page));

		public async Task<IActionResult> Details(
			string id, string? startDate, string? endDate, int page = 1)
		{
			DetailsAccountViewModel viewModel;

			try
			{
				if (startDate == null || endDate == null)
				{
					viewModel = await accountService.GetAccountDetailsViewModel(
						id, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, page);
				}
				else
				{
					viewModel = await accountService.GetAccountDetailsViewModel(
						id, DateTime.Parse(startDate), DateTime.Parse(endDate), page);
				}
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			viewModel.Routing.Area = "Admin";
			viewModel.Routing.ReturnUrl = "/Admin/Account/Details/" + id;
			ViewBag.ModelId = id;

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Details(
			string id, [Bind("StartDate,EndDate")] DetailsAccountViewModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				try
				{
					await accountService.SetAccountDetailsViewModelForReturn(id, inputModel);

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
				//Not supposed to have null values!
				viewModel = await accountService.GetAccountDetailsViewModel(id,
					inputModel.StartDate ?? DateTime.UtcNow.AddMonths(-1),
					inputModel.EndDate ?? DateTime.UtcNow);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			viewModel.Routing.Area = "Admin";
			viewModel.Routing.ReturnUrl = "/Admin/Account/Details/" + id;
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
			//TODO: Remove Create and delete buttons for admin
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
				await PrepareModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				string ownerId = await accountService.GetOwnerId(id);

				await accountService.EditAccount(id, inputModel, ownerId);

				TempData["successMsg"] = "You successfully edited user's account!";

				return LocalRedirect(returnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(
					nameof(inputModel.Name), 
					$"You already have Account with {inputModel.Name} name.");

				await PrepareModelForReturn(inputModel);

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		private async Task PrepareModelForReturn(AccountFormModel model)
		{
			AccountFormModel emptyFormModel =
				await accountService.GetEmptyAccountFormModel(this.User.Id());

			model.AccountTypes = emptyFormModel.AccountTypes;
			model.Currencies = emptyFormModel.Currencies;
		}
	}
}