using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
    [Authorize(Roles = UserRoleName)]
	public class TransactionsController : Controller
	{
		private readonly IAccountsService accountsService;

		public TransactionsController(IAccountsService accountsService)
			=> this.accountsService = accountsService;

		public async Task<IActionResult> All()
		{
			var inputModel = new DateFilterModel
			{
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow
			};

			return View(await accountsService
				.GetUserTransactionsViewModel(User.Id(), inputModel));
		}

		[HttpPost]
		public async Task<IActionResult> All(DateFilterModel inputModel)
		{
			if (!ModelState.IsValid)
				return View(new UserTransactionsViewModel
				{
					StartDate = inputModel.StartDate ?? new DateTime(),
					EndDate = inputModel.EndDate ?? new DateTime()
				});

			return View(await accountsService
				.GetUserTransactionsViewModel(User.Id(), inputModel));
		}

		public async Task<IActionResult> Create()
		{
			TransactionFormModel viewModel =
				await accountsService.GetEmptyTransactionFormModel(User.Id());

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				await accountsService.PrepareTransactionFormModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				string newTransactionId =
					await accountsService.CreateTransaction(User.Id(), inputModel);

				TempData["successMsg"] = "You create a new transaction successfully!";

				return RedirectToAction(nameof(TransactionDetails), new { id = newTransactionId });
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string id, string? returnUrl = null)
		{
			try
			{
				await accountsService.DeleteTransaction(id, User.Id());
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your transaction was successfully deleted!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);
			else
				return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> TransactionDetails(string id)
		{
			try
			{
				return View(await accountsService.GetTransactionViewModel(id));
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
					await accountsService.GetFulfilledTransactionFormModel(id);

				if (User.Id() != viewModel.OwnerId)
					return Unauthorized();

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
			if (User.Id() != inputModel.OwnerId)
				return Unauthorized();

			if (!ModelState.IsValid)
			{
				await accountsService.PrepareTransactionFormModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				await accountsService.EditTransaction(id, inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			TempData["successMsg"] = "Your transaction was successfully edited!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);

			return RedirectToAction(nameof(TransactionDetails), new { id });
		}
	}
}
