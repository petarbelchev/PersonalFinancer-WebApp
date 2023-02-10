using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Account.Models;

namespace PersonalFinancer.Services.Account
{
    public interface IAccountService
	{
		Task<IEnumerable<AccountViewModel>> AllAccounts(string userId);

		Task<IEnumerable<AccountViewModelExtended>> AllAccountsWithData(string userId);

		Task<IEnumerable<AccountTypeViewModel>> AllAccountTypes(string userId);

		Task CreateAccount(string userId, CreateAccountFormModel accountModel);

		Task ChangeBalance(int accountId, decimal amount, TransactionType transactionType);

		Task<bool> IsOwner(string userId, int accountId);

		Task Add(TransactionFormModel transactionFormModel);

		Task Delete(int id);

		Task Edit(TransactionFormModel transactionFormModel);

		Task<TransactionFormModel?> FindTransactionById(int id);

		Task<IEnumerable<TransactionViewModel>> LastFiveTransactions(string userId);
	}
}
