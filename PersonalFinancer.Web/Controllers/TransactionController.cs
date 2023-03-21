using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.Transactions;
using PersonalFinancer.Services.Transactions.Models;
using PersonalFinancer.Web.Infrastructure;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize(Roles = UserRoleName)]
	public class TransactionController : Controller
	{
		private readonly ITransactionsService transactionsService;

		public TransactionController(ITransactionsService transactionsService)
		{
			this.transactionsService = transactionsService;
		}

		//TODO: Use DateFilterModel!
		public async Task<IActionResult> All(string? startDate, string? endDate, int page = 1)
		{
			var viewModel = new UserTransactionsExtendedViewModel();

			viewModel.Page = page;

			if (startDate == null || endDate == null)
			{
				viewModel.StartDate = DateTime.UtcNow.AddMonths(-1);
				viewModel.EndDate = DateTime.UtcNow;
			}
			else
			{
				viewModel.StartDate = DateTime.Parse(startDate);
				viewModel.EndDate = DateTime.Parse(endDate);
			}

			try
			{
				await transactionsService.GetUserTransactionsExtendedViewModel(User.Id(), viewModel);

				return View(viewModel);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(viewModel.EndDate), ex.Message);

				return View(viewModel);
			}
		}

		[HttpPost]
		public async Task<IActionResult> All(DateFilterModel dateModel)
		{
			if (!ModelState.IsValid)
				return View(dateModel);

			var viewModel = new UserTransactionsExtendedViewModel 
			{ 
				StartDate = dateModel.StartDate,
				EndDate = dateModel.EndDate
			};

			try
			{
				await transactionsService.GetUserTransactionsExtendedViewModel(User.Id(), viewModel);

				return View(viewModel);
			}
			catch (ArgumentException ex)
			{
				ModelState.AddModelError(nameof(viewModel.EndDate), ex.Message);

				return View(viewModel);
			}
		}

		public async Task<IActionResult> Create()
		{
			TransactionFormModel viewModel =
				await transactionsService.GetEmptyTransactionFormModel(User.Id());

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				await PrepareModelForReturn(inputModel);

				return View(inputModel);
			}

			try
			{
				string newTransactionId =
					await transactionsService.CreateTransaction(User.Id(), inputModel);

				TempData["successMsg"] = "You create a new transaction successfully!";

				return RedirectToAction(nameof(Details), new { id = newTransactionId });
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
				await transactionsService.DeleteTransaction(id, User.Id());
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

		public async Task<IActionResult> Details(string id)
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

			TempData["successMsg"] = "Your transaction was successfully edited!";

			if (returnUrl != null)
				return LocalRedirect(returnUrl);

			return RedirectToAction(nameof(Details), new { id });
		}

		private async Task PrepareModelForReturn(TransactionFormModel model)
		{
			TransactionFormModel emptyFormModel =
				await transactionsService.GetEmptyTransactionFormModel(User.Id());

			model.Categories = emptyFormModel.Categories;
			model.Accounts = emptyFormModel.Accounts;
		}
	}
}
