namespace PersonalFinancer.Web.Controllers.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Services.User;
    using PersonalFinancer.Web.Infrastructure.Extensions;
    using PersonalFinancer.Web.Models.Shared;
    using PersonalFinancer.Web.Models.Transaction;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using static PersonalFinancer.Data.Constants.RoleConstants;

    [Authorize]
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsApiController : BaseApiController
    {
        private readonly IAccountsService accountService;
        private readonly IUsersService usersService;

        public TransactionsApiController(
            IAccountsService accountsService,
            IUsersService usersService)
        {
            this.accountService = accountsService;
            this.usersService = usersService;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction([Required] Guid? id)
        {
            if (!this.ModelState.IsValid)
                return this.BadRequest(this.GetErrors(this.ModelState.Values));

            try
            {
                decimal newBalance = await this.accountService
                    .DeleteTransaction(id, this.User.Id(), this.User.IsAdmin());

                return this.Ok(new { newBalance });
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }

        [Authorize(Roles = UserRoleName)]
        [HttpPost]
        public async Task<IActionResult> GetUserTransactions(UserTransactionsApiInputModel inputModel)
        {
            bool isStartDateValid = DateTime.TryParse(
                inputModel.StartDate, null, DateTimeStyles.None, out DateTime startDate);

            bool isEndDateValid = DateTime.TryParse(
                inputModel.EndDate, null, DateTimeStyles.None, out DateTime endDate);

            if (!this.ModelState.IsValid || !isStartDateValid || !isEndDateValid || startDate > endDate)
                return this.BadRequest();

            if (inputModel.Id != this.User.Id())
                return this.Unauthorized();

            try
            {
                TransactionsServiceModel userTransactions = await this.usersService
                    .GetUserTransactions(inputModel.Id, startDate, endDate, inputModel.Page);

                var userModel = new TransactionsViewModel
                {
                    Transactions = userTransactions.Transactions
                };
                userModel.Pagination.Page = inputModel.Page;
                userModel.Pagination.TotalElements = userTransactions.TotalTransactionsCount;
                userModel.TransactionDetailsUrl = "/Transactions/TransactionDetails/";

                return this.Ok(userModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }
    }
}
