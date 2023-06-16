namespace PersonalFinancer.Services.Transactions
{
	using Microsoft.EntityFrameworkCore;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Services.Transactions.Models;
	using System;
	using System.Threading.Tasks;
	using static PersonalFinancer.Services.Infrastructure.Constants;

	public class TransactionsInfoService : ITransactionsInfoService
	{
		private readonly IEfRepository<Transaction> transactionsRepo;

		public TransactionsInfoService(IEfRepository<Transaction> repo)
			=> this.transactionsRepo = repo;

		public async Task<TransactionsServiceModel> GetAccountTransactionsAsync(
			Guid accountId, DateTime startDate, DateTime endDate, int page = 1)
		{
			IQueryable<Transaction> query = this.transactionsRepo.All()
				.Where(t => t.AccountId == accountId && !t.Account.IsDeleted);

			return await this.GetTransactions(query, startDate, endDate, page);
		}

		public async Task<TransactionsServiceModel> GetUserTransactionsAsync(
			Guid userId, DateTime startDate, DateTime endDate, int page = 1)
		{
			IQueryable<Transaction> query = this.transactionsRepo.All()
				.Where(t => t.OwnerId == userId);

			return await this.GetTransactions(query, startDate, endDate, page);
		}

		private async Task<TransactionsServiceModel> GetTransactions(
			 IQueryable<Transaction> query, DateTime startDate, DateTime endDate, int page = 1)
		{
			DateTime startDateUtc = startDate.ToUniversalTime();
			DateTime endDateUtc = endDate.ToUniversalTime();

			return new TransactionsServiceModel()
			{
				StartDate = startDate,
				EndDate = endDate,
				Transactions = await query
					.Where(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
					.OrderByDescending(t => t.CreatedOn)
					.Skip(PaginationConstants.TransactionsPerPage * (page - 1))
					.Take(PaginationConstants.TransactionsPerPage)
					.Select(t => new TransactionTableServiceModel
					{
						Id = t.Id,
						Amount = t.Amount,
						AccountCurrencyName = t.Account.Currency.Name,
						CreatedOn = t.CreatedOn.ToLocalTime(),
						CategoryName = t.Category.Name + (t.Category.IsDeleted ?
							" (Deleted)"
							: string.Empty),
						TransactionType = t.TransactionType.ToString(),
						Reference = t.Reference
					})
					.ToArrayAsync(),
				TotalTransactionsCount = await query
					.CountAsync(t => t.CreatedOn >= startDateUtc && t.CreatedOn <= endDateUtc)
			};
		}
	}
}
