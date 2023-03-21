﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionController : Controller
	{
		private readonly ITransactionsService transactionsService;

		public TransactionController(ITransactionsService transactionsService)
		{
			this.transactionsService = transactionsService;
		}

		public async Task<IActionResult> EditTransaction(string id)
		{
			try
			{
				TransactionFormModel viewModel = 
					await transactionsService.GetFulfilledTransactionFormModel(id);

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditTransaction(
			string id, TransactionFormModel inputModel, string? returnUrl)
		{
			if (!ModelState.IsValid)
			{
				await PrepareModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				await transactionsService.EditTransaction(id, inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "You successfully edit User's transaction!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);

			return RedirectToAction("Details", "Account", new { id = inputModel.AccountId });
		}

		private async Task PrepareModelForReturn(TransactionFormModel model)
		{
			TransactionFormModel emptyFormModel =
				await transactionsService.GetEmptyTransactionFormModel(model.OwnerId);

			model.Categories = emptyFormModel.Categories;
			model.Accounts = emptyFormModel.Accounts;
		}
	}
}
