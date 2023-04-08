namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Accounts;
	using Services.Accounts.Models;
	using Services.Shared.Models;

	using Web.Infrastructure;
	using Web.Models.Accounts;
	using Web.Models.Shared;
	
	using static Data.Constants.RoleConstants;

	[Authorize(Roles = UserRoleName)]
	public class TransactionsController : Controller
	{
		private readonly IAccountsService accountsService;
		private readonly IMapper mapper;
		private readonly IControllerService controllerService;

		public TransactionsController(
			IAccountsService accountsService,
			IMapper mapper,
			IControllerService controllerService)
		{
			this.accountsService = accountsService;
			this.mapper = mapper;
			this.controllerService = controllerService;
		}

		public async Task<IActionResult> All()
		{
			var viewModel = new UserTransactionsViewModel();

			var inputDTO = new UserTransactionsInputDTO
			{
				Id = User.Id(),
				StartDate = DateTime.UtcNow.AddMonths(-1),
				EndDate = DateTime.UtcNow,
				ElementsPerPage = viewModel.Pagination.ElementsPerPage
			};

			UserTransactionsOutputDTO transactionsData =
				await accountsService.GetUserTransactions(inputDTO);

			viewModel = mapper.Map<UserTransactionsViewModel>(transactionsData);
			viewModel.Pagination.TotalElements = transactionsData.AllTransactionsCount;

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> All(DateFilterInputModel inputModel)
		{
			if (!ModelState.IsValid)
				return View(new UserTransactionsViewModel
				{
					StartDate = inputModel.StartDate,
					EndDate = inputModel.EndDate
				});

			var viewModel = new UserTransactionsViewModel();

			var inputDTO = new UserTransactionsInputDTO
			{
				Id = User.Id(),
				StartDate = inputModel.StartDate,
				EndDate = inputModel.EndDate,
				ElementsPerPage = viewModel.Pagination.ElementsPerPage
			};

			UserTransactionsOutputDTO transactionsData =
				await accountsService.GetUserTransactions(inputDTO);

			viewModel = mapper.Map<UserTransactionsViewModel>(transactionsData);
			viewModel.Pagination.TotalElements = transactionsData.AllTransactionsCount;

			return View(viewModel);
		}

		public async Task<IActionResult> Create()
		{
			EmptyTransactionFormDTO formData =
				await accountsService.GetEmptyTransactionForm(User.Id());

			var viewModel = mapper.Map<TransactionFormModel>(formData);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TransactionFormModel inputModel)
		{
			try
			{
				if (inputModel.OwnerId != User.Id())
					throw new InvalidOperationException();

				if (!ModelState.IsValid)
				{
					await controllerService.PrepareTransactionFormModelForReturn(inputModel);

					return View(inputModel);
				}

				var inputDTO = mapper.Map<CreateTransactionInputDTO>(inputModel);

				string newTransactionId = await accountsService.CreateTransaction(inputDTO);

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
				TransactionDetailsDTO transactionData =
					await accountsService.GetTransactionDetails(id);

				TransactionDetailsViewModel viewModel =
					mapper.Map<TransactionDetailsViewModel>(transactionData);

				return View(viewModel);
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
				FulfilledTransactionFormDTO transactionFormDTO =
					await accountsService.GetFulfilledTransactionForm(id);

				if (User.Id() != transactionFormDTO.OwnerId)
					return Unauthorized();

				var viewModel = mapper.Map<TransactionFormModel>(transactionFormDTO);

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
				await controllerService.PrepareTransactionFormModelForReturn(inputModel);
				return View(inputModel);
			}

			try
			{
				var inputDTO = mapper.Map<EditTransactionInputDTO>(inputModel);
				await accountsService.EditTransaction(inputDTO);
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
