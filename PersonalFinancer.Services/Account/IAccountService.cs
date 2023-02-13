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

		Task ChangeBalance(Guid accountId, decimal amount, TransactionType transactionType);

		Task<bool> IsAccountOwner(string userId, Guid accountId);

		Task CreateTransaction(TransactionServiceModel transactionFormModel);

		Task DeleteTransactionById(Guid id);
		
		Task DeleteAccountById(Guid id, bool transactionsDelete);

		Task EditTransaction(TransactionServiceModel transactionFormModel);

		Task<TransactionServiceModel?> GetTransactionById(Guid id);

		Task<IEnumerable<TransactionViewModel>> GetTransactionsByAccountId(Guid id);

		Task<IEnumerable<TransactionViewModel>> LastFiveTransactions(string userId);

		Task<AccountViewModel> GetAccountById(Guid id);

		Task<AccountViewModelExtended> GetAccountByIdExtended(Guid id);

		Task<Dictionary<string, CashFlowViewModel>> GetCashFlow(string userId);
	}
}
