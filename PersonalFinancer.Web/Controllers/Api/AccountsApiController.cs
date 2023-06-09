namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Account;
    using PersonalFinancer.Web.Models.Shared;
    using System.Globalization;
    using static PersonalFinancer.Data.Constants;

    [Authorize]
    [Route("api/accounts")]
    [ApiController]
    public class AccountsApiController : ControllerBase
    {
        private readonly IAccountsService accountService;

        public AccountsApiController(IAccountsService accountService)
            => this.accountService = accountService;

        [Authorize(Roles = RoleConstants.AdminRoleName)]
        [HttpGet("{page}")]
        public async Task<IActionResult> GetAccounts(int page)
        {
            UsersAccountsCardsServiceModel usersCardsData =
                await this.accountService.GetAccountsCardsData(page);

            var usersCardsModel = new UsersAccountCardsViewModel
            {
                Accounts = usersCardsData.Accounts
            };

            usersCardsModel.Pagination.TotalElements = usersCardsData.TotalUsersAccountsCount;

            return this.Ok(usersCardsModel);
        }

        [Authorize(Roles = RoleConstants.AdminRoleName)]
        [HttpGet("cashflow")]
        public async Task<IActionResult> GetAccountsCashFlow()
            => this.Ok(await this.accountService.GetCurrenciesCashFlow());

        [HttpPost("transactions")]
        public async Task<IActionResult> GetAccountTransactions(AccountTransactionsInputModel inputModel)
        {
            // TODO: Try to use input model with DateTime props

            bool isStartDateValid = DateTime.TryParse(
                inputModel.StartDate, null, DateTimeStyles.None, out DateTime startDate);

            bool isEndDateValid = DateTime.TryParse(
                inputModel.EndDate, null, DateTimeStyles.None, out DateTime endDate);

            if (!this.ModelState.IsValid || !isStartDateValid || !isEndDateValid || startDate > endDate)
                return this.BadRequest();

            if (!this.User.IsAdmin() && inputModel.OwnerId != this.User.Id())
                return this.Unauthorized();

            try
            {
                TransactionsServiceModel accountTransactions = await this.accountService
                    .GetAccountTransactions(inputModel.Id, startDate, endDate, inputModel.Page);

                var viewModel = new TransactionsViewModel
                {
                    Transactions = accountTransactions.Transactions
                };

                viewModel.Pagination.TotalElements = accountTransactions.TotalTransactionsCount;
                viewModel.Pagination.Page = inputModel.Page;
                viewModel.TransactionDetailsUrl =
                    $"{(this.User.IsAdmin() ? "/Admin" : string.Empty)}/Transactions/TransactionDetails/";

                return this.Ok(viewModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }
    }
}
