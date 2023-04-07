using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Web.Models.Accounts;
using PersonalFinancer.Web.Models.Shared;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class AccountsController : Controller
	{
		private readonly IAccountsService accountService;
		private readonly IMapper mapper;

		public AccountsController(
			IAccountsService accountService,
			IMapper mapper)
		{
			this.accountService = accountService;
			this.mapper = mapper;
		}

		public async Task<IActionResult> Index(int page = 1)
		{
			var viewModel = new UsersAccountCardsViewModel();

			AccountCardsOutputDTO accountCardsData = await accountService
				.GetUsersAccountCards(page, viewModel.Pagination.ElementsPerPage);
			viewModel.Accounts = accountCardsData.Accounts
				.Select(ac => mapper.Map<AccountCardExtendedViewModel>(ac)).ToArray();
			viewModel.Pagination.Page = accountCardsData.Page;
			viewModel.Pagination.TotalElements = accountCardsData.AllAccountsCount;

			return View(viewModel);
		}

		public async Task<IActionResult> AccountDetails(string id)
		{
			// ApiTransactionsEndpoint = HostConstants.ApiAccountTransactionsUrl,

			try
			{
				var inputDTO = new AccountDetailsInputDTO
				{
					Id = id,
					StartDate = DateTime.UtcNow.AddMonths(-1),
					EndDate = DateTime.UtcNow
				};

				AccountDetailsOutputDTO accountData =
					await accountService.GetAccountDetails(inputDTO);

				var viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
				//viewModel.Pagination.ElementsName = "transactions";
				viewModel.Pagination.TotalElements = accountData.AllTransactionsCount;
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
			AccountDetailsOutputDTO accountData;
			AccountDetailsViewModel viewModel;
			var inputDTO = new AccountDetailsInputDTO
			{
				Id = inputModel.Id,
				StartDate = inputModel.StartDate ?? new DateTime(),
				EndDate = inputModel.EndDate ?? new DateTime()
			};

			try
			{
				if (!ModelState.IsValid)
				{
					accountData = await accountService.GetAccountDetailsForReturn(inputDTO);
					viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
				}
				else
				{
					accountData = await accountService.GetAccountDetails(inputDTO);
					viewModel = mapper.Map<AccountDetailsViewModel>(accountData);
					//viewModel.Pagination.ElementsName = "transactions";
					viewModel.Pagination.TotalElements = accountData.AllTransactionsCount;
					viewModel.Routing.Area = "Admin";
					viewModel.Routing.ReturnUrl = "/Admin/Accounts/AccountDetails/" + inputModel.Id;
				}

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				DeleteAccountDTO accountData =
					await accountService.GetDeleteAccountData(id);

				var viewModel = mapper.Map<DeleteAccountViewModel>(accountData);

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
				EditAccountFormDTO accountData = await accountService.GetAccountForm(id);

				var viewModel = mapper.Map<CreateAccountFormModel>(accountData);

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditAccount(EditAccountFormModel inputModel)
		{
			EditAccountFormDTO accountData;

			try
			{
				if (!ModelState.IsValid)
				{
					await PrepareViewModelForReturn(inputModel);

					return View(inputModel);
				}

				accountData = mapper.Map<EditAccountFormDTO>(inputModel);
				await accountService.EditAccount(accountData);

				TempData["successMsg"] = "You successfully edited user's account!";

				return LocalRedirect(inputModel.ReturnUrl);
			}
			catch (ArgumentException)
			{
				ModelState.AddModelError(
					nameof(inputModel.Name),
					$"The User already have Account with {inputModel.Name} name.");

				await PrepareViewModelForReturn(inputModel);

				return View(inputModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		private async Task PrepareViewModelForReturn<T>(T inputModel)
			where T : IAccountFormModel
		{
			CreateAccountFormDTO accountData =
				await accountService.GetEmptyAccountForm(inputModel.OwnerId);
			inputModel.AccountTypes = accountData.AccountTypes
				.Select(at => mapper.Map<AccountTypeViewModel>(at));
			inputModel.Currencies = accountData.Currencies
				.Select(c => mapper.Map<CurrencyViewModel>(c));
		}
	}
}
