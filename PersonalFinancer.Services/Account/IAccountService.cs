namespace PersonalFinancer.Services.Account
{
	using Models;
	using Data.Enums;

	public interface IAccountService
	{
		Task<IEnumerable<AccountViewModel>> AllAccounts(string userId);

		Task<IEnumerable<AccountViewModelExtended>> AllAccountsWithData(string userId);

		Task<IEnumerable<AccountTypeViewModel>> AllAccountTypes(string userId);

		Task CreateAccount(string userId, CreateAccountFormModel accountModel);

		Task ChangeBalance(int accountId, decimal amount, TransactionType transactionType);

		Task<bool> IsAccountOwner(string userId, int accountId);

		Task CreateTransaction(TransactionServiceModel transactionFormModel);

		Task DeleteTransactionById(int id);

		Task EditTransaction(TransactionServiceModel transactionFormModel);

		Task<TransactionServiceModel?> GetTransactionById(int id);

		Task<IEnumerable<TransactionViewModel>> GetTransactionsByAccountId(int id);

		Task<IEnumerable<TransactionViewModel>> LastFiveTransactions(string userId);

		Task<AccountViewModelExtended> GetAccountById(int id);
	}
}
