using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class TransactionsController : Controller
	{
		private readonly ITransactionsService transactionsService;

		public TransactionsController(ITransactionsService transactionsService)
			=> this.transactionsService = transactionsService;
		
		public async Task<IActionResult> TransactionDetails(string id)
		{
			try
			{
				return View(await transactionsService.GetTransactionViewModel(id));
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
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

			return RedirectToAction("AccountDetails", "Accounts", new { id = inputModel.AccountId });
		}

		private async Task PrepareModelForReturn(TransactionFormModel model)
		{
			TransactionFormModel emptyFormModel =
				await transactionsService.GetEmptyTransactionFormModel(model.OwnerId);

			model.UserCategories = emptyFormModel.UserCategories;
			model.UserAccounts = emptyFormModel.UserAccounts;
		}
	}
}
