using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Infrastructure.Extensions;
using PersonalFinancer.Web.Models.Shared;
using PersonalFinancer.Web.Models.Transaction;
using static PersonalFinancer.Web.Infrastructure.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize]
	public class TransactionsController : Controller
	{
		protected readonly IAccountsService accountsService;
		protected readonly IUsersService usersService;
		protected readonly IMapper mapper;

		public TransactionsController(
			IAccountsService accountsService,
			IUsersService usersService,
			IMapper mapper)
		{
			this.accountsService = accountsService;
			this.usersService = usersService;
			this.mapper = mapper;
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> All()
		{
			DateTime startDate = DateTime.Now.AddMonths(-1);
			DateTime endDate = DateTime.Now;

			UserTransactionsViewModel viewModel =
				await PrepareUserTransactionsViewModel(User.Id(), startDate, endDate);

			return View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> All(DateFilterModel inputModel)
		{
			if (!ModelState.IsValid)
				return View(mapper.Map<UserTransactionsViewModel>(inputModel));

			UserTransactionsViewModel viewModel = await PrepareUserTransactionsViewModel(
				User.Id(), inputModel.StartDate, inputModel.EndDate);

			return View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			UserAccountsAndCategoriesServiceModel userData =
				await usersService.GetUserAccountsAndCategories(User.Id());

			var viewModel = mapper.Map<TransactionFormModel>(userData);
			viewModel.CreatedOn = DateTime.Now;

			return View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				await PrepareTransactionFormModelForReturn(inputModel);

				return View(inputModel);
			}

			if (inputModel.OwnerId != User.Id())
				return BadRequest();

			try
			{
				var serviceModel = mapper.Map<TransactionFormShortServiceModel>(inputModel);
				string newTransactionId = await accountsService.CreateTransaction(serviceModel);
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
				await accountsService.DeleteTransaction(id, User.Id(), User.IsAdmin());

				TempData["successMsg"] = User.IsAdmin() ?
					"You successfully delete a user's transaction!"
					: "Your transaction was successfully deleted!";
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			if (returnUrl != null)
				return LocalRedirect(returnUrl);
			else
				return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> TransactionDetails(string id)
		{
			try
			{
				return View(await accountsService
					.GetTransactionDetails(id, User.Id(), User.IsAdmin()));
			}
			catch (ArgumentException)
			{
				return Unauthorized();
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
				TransactionFormServiceModel transactionData =
					await accountsService.GetTransactionFormData(id);

				if (!User.IsAdmin() && User.Id() != transactionData.OwnerId)
					return Unauthorized();

				var viewModel = mapper.Map<TransactionFormModel>(transactionData);

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditTransaction(string id, TransactionFormModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				await PrepareTransactionFormModelForReturn(inputModel);

				return View(inputModel);
			}

			string ownerId = User.IsAdmin() ?
				await accountsService.GetOwnerId(inputModel.AccountId)
				: User.Id();

			if (inputModel.OwnerId != ownerId)
				return Unauthorized();

			var serviceModel = mapper.Map<TransactionFormShortServiceModel>(inputModel);

			try
			{
				await accountsService.EditTransaction(id, serviceModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			if (User.IsAdmin())
			{
				TempData["successMsg"] = "You successfully edit User's transaction!";
				return RedirectToAction("AccountDetails", "Accounts", new { id = inputModel.AccountId });
			}
			else
			{
				TempData["successMsg"] = "Your transaction was successfully edited!";
				return RedirectToAction(nameof(TransactionDetails), new { id });
			}
		}

		private async Task PrepareTransactionFormModelForReturn(TransactionFormModel formModel)
		{
			UserAccountsAndCategoriesServiceModel userData =
				await usersService.GetUserAccountsAndCategories(formModel.OwnerId);

			formModel.UserCategories = userData.UserCategories;
			formModel.UserAccounts = userData.UserAccounts;
		}

		private async Task<UserTransactionsViewModel> PrepareUserTransactionsViewModel(
			string userId, DateTime startDate, DateTime endDate)
		{
			TransactionsServiceModel userTransactions =
				await usersService.GetUserTransactions(userId, startDate, endDate);

			var viewModel = mapper.Map<UserTransactionsViewModel>(userTransactions);
			viewModel.Id = userId;
			viewModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;

			return viewModel;
		}
	}
}
