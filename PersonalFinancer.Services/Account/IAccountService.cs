using PersonalFinancer.Services.Account.Models;

namespace PersonalFinancer.Services.Account
{
    public interface IAccountService
	{
		Task AddTransaction(CreateTransactionFormModel transactionFormModel);

		Task<IEnumerable<AccountViewModel>> AllAccounts(string userId);

		Task<IEnumerable<AccountViewModelExtended>> AllAccountsWithData(string userId);

		Task<IEnumerable<AccountTypeViewModel>> AllAccountTypes(string userId);

		Task CreateAccount(string userId, CreateAccountFormModel accountModel);

		Task<IEnumerable<TransactionViewModel>> LastFiveTransactions(string userId);
	}
}
