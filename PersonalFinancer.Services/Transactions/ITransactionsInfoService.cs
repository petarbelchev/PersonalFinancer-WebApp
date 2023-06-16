namespace PersonalFinancer.Services.Transactions
{
    using PersonalFinancer.Services.Transactions.Models;
    using System;
    using System.Threading.Tasks;

    public interface ITransactionsInfoService
	{
		Task<TransactionsServiceModel> GetAccountTransactionsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, int page = 1);

		Task<TransactionsServiceModel> GetUserTransactionsAsync(
			Guid userId, DateTime startDate, DateTime endDate, int page = 1);
	}
}
