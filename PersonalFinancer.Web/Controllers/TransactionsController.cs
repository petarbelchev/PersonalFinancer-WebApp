namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Transaction;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Authorize]
	public class TransactionsController : Controller
	{
		protected readonly IAccountsUpdateService accountsUpdateService;
		protected readonly IAccountsInfoService accountsInfoService;
		protected readonly IUsersService usersService;
		protected readonly IMapper mapper;

		public TransactionsController(
			IAccountsUpdateService accountsUpdateService,
			IAccountsInfoService accountsInfoService,
			IUsersService usersService,
			IMapper mapper)
		{
			this.accountsUpdateService = accountsUpdateService;
			this.accountsInfoService = accountsInfoService;
			this.usersService = usersService;
			this.mapper = mapper;
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Create()
		{
			var viewModel = new CreateEditTransactionViewModel() { OwnerId = this.User.IdToGuid() };
			await this.PrepareAccountsAndCategoriesAsync(viewModel);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(CreateEditTransactionInputModel inputModel)
		{
			if (inputModel.OwnerId != this.User.IdToGuid())
				return this.BadRequest();

			if (!this.ModelState.IsValid)
			{
				CreateEditTransactionViewModel viewModel =
					this.mapper.Map<CreateEditTransactionViewModel>(inputModel);

				await this.PrepareAccountsAndCategoriesAsync(viewModel);

				return this.View(viewModel);
			}

			try
			{
				CreateEditTransactionInputDTO dto = this.mapper.Map<CreateEditTransactionInputDTO>(inputModel);
				Guid newTransactionId = await this.accountsUpdateService.CreateTransactionAsync(dto);
				this.TempData[ResponseMessages.TempDataKey] = ResponseMessages.CreatedTransaction;

				return this.RedirectToAction(nameof(Details), new { id = newTransactionId });
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}
		}

		[HttpPost]
		[NoHtmlSanitizing]
		public async Task<IActionResult> Delete([Required] Guid id, string? returnUrl = null)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			try
			{
				await this.accountsUpdateService.DeleteTransactionAsync(id, this.User.IdToGuid(), this.User.IsAdmin());

				this.TempData[ResponseMessages.TempDataKey] = this.User.IsAdmin()
					? ResponseMessages.AdminDeletedUserTransaction
					: ResponseMessages.DeletedTransaction;
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return returnUrl != null
				? this.LocalRedirect(returnUrl)
				: this.RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Details([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			TransactionDetailsDTO viewModel;

			try
			{
				viewModel = await this.accountsInfoService
					.GetTransactionDetailsAsync(id, this.User.IdToGuid(), this.User.IsAdmin());
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.View(viewModel);
		}

		public async Task<IActionResult> Edit([Required] Guid id)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			try
			{
				CreateEditTransactionOutputDTO transactionDTO = await this.accountsInfoService
					.GetTransactionFormDataAsync(id, this.User.IdToGuid(), this.User.IsAdmin());

				CreateEditTransactionViewModel viewModel = this.mapper.Map<CreateEditTransactionViewModel>(transactionDTO);

				return this.View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit([Required] Guid id, CreateEditTransactionInputModel inputModel)
		{
			if (!this.User.IsAdmin() && this.User.IdToGuid() != inputModel.OwnerId)
				return this.BadRequest();

			try
			{
				if (!this.ModelState.IsValid)
				{
					CreateEditTransactionViewModel viewModel =
						this.mapper.Map<CreateEditTransactionViewModel>(inputModel);

					await this.PrepareAccountsAndCategoriesAsync(viewModel);

					return this.View(viewModel);
				}

				CreateEditTransactionInputDTO dto =
					this.mapper.Map<CreateEditTransactionInputDTO>(inputModel);

				await this.accountsUpdateService.EditTransactionAsync(id, dto);
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			this.TempData[ResponseMessages.TempDataKey] = this.User.IsAdmin()
				? ResponseMessages.AdminEditedUserTransaction
				: ResponseMessages.EditedTransaction;

			return this.RedirectToAction(nameof(Details), new { id });
		}

		[Authorize(Roles = UserRoleName)]
		public async Task<IActionResult> Index()
		{
			var filterDTO = new TransactionsFilterDTO
			{
				UserId = this.User.IdToGuid(),
				FromLocalTime = DateTime.Now.AddMonths(-1),
				ToLocalTime = DateTime.Now
			};

			UserTransactionsViewModel viewModel =
				await this.PrepareUserTransactionsViewModelAsync(filterDTO);

			return this.View(viewModel);
		}

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		[NoHtmlSanitizing]
		public async Task<IActionResult> Index(UserTransactionsInputModel inputModel)
		{
			Guid userId = this.User.IdToGuid();
			UserTransactionsViewModel viewModel;

			if (!this.ModelState.IsValid)
			{
				viewModel = this.mapper.Map<UserTransactionsViewModel>(inputModel);
				viewModel.UserId = userId;
				UserDropdownDTO dropdownDTO = await this.usersService.GetUserDropdownDataAsync(userId);
				this.mapper.Map(dropdownDTO, viewModel);

				return this.View(viewModel);
			}

			TransactionsFilterDTO filterDTO = this.mapper.Map<TransactionsFilterDTO>(inputModel);
			filterDTO.UserId = userId;
			viewModel = await this.PrepareUserTransactionsViewModelAsync(filterDTO);

			return this.View(viewModel);
		}

		/// <exception cref="InvalidOperationException">When the owner ID is invalid.</exception>
		private async Task PrepareAccountsAndCategoriesAsync(CreateEditTransactionViewModel formModel)
		{
			Guid ownerId = formModel.OwnerId ?? throw new InvalidOperationException(
				string.Format(ExceptionMessages.NotNullableProperty, formModel.OwnerId));

			AccountsAndCategoriesDropdownDTO accountsAndCategoriesDTO =
				await this.usersService.GetUserAccountsAndCategoriesDropdownDataAsync(ownerId);

			this.mapper.Map(accountsAndCategoriesDTO, formModel);
		}

		private async Task<UserTransactionsViewModel> PrepareUserTransactionsViewModelAsync(TransactionsFilterDTO filterDTO)
		{
			TransactionsPageDTO transactionsDTO =
				await this.usersService.GetUserTransactionsPageDataAsync(filterDTO);
			var viewModel = new UserTransactionsViewModel(
				transactionsDTO.TotalTransactionsCount, filterDTO.Page);
			this.mapper.Map(filterDTO, viewModel);
			this.mapper.Map(transactionsDTO, viewModel);

			return viewModel;
		}
	}
}
