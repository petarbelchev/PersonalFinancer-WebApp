using Microsoft.EntityFrameworkCore;
using PersonalFinancer.Data.Enums;
using PersonalFinancer.Services.Account.Models;
using PersonalFinancer.Web.Data;

namespace PersonalFinancer.Services.Account
{
    public class AccountService : IAccountService
	{
		private readonly PersonalFinancerDbContext data;

		public AccountService(PersonalFinancerDbContext context)
			=> this.data = context;

		public async Task<IEnumerable<AccountViewModel>> AllAccounts(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId)
				.Select(a => new AccountViewModel
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToArrayAsync();
		}
		
		public async Task<IEnumerable<AccountViewModelExtended>> AllAccountsWithData(string userId)
		{
			return await data.Accounts
				.Where(a => a.OwnerId == userId)
				.Select(a => new AccountViewModelExtended
				{
					Id = a.Id,
					Name = a.Name,
					Balance = a.Balance,
					Currency = a.Currency.Name
				})
				.ToArrayAsync();
		}

		public async Task<IEnumerable<AccountTypeViewModel>> AllAccountTypes(string userId)
		{
			return await data.AccountTypes
				.Where(a => a.UserId == null || a.UserId == userId)
				.Select(a => new AccountTypeViewModel
				{
					Id = a.Id,
					Name = a.Name
				})
				.ToArrayAsync();
		}

		public async Task AddTransaction(CreateTransactionFormModel transactionFormModel)
		{
			var newTransaction = new Data.Models.Transaction()
			{
				Amount = transactionFormModel.Amount,
				AccountId = transactionFormModel.AccountId,
				CategoryId = transactionFormModel.CategoryId,
				TransactionType = transactionFormModel.TransactionType,
				Refference = transactionFormModel.PaymentRefference
			};

			var account = await data.Accounts.FirstAsync(a => a.Id == newTransaction.AccountId);
			if (transactionFormModel.TransactionType == TransactionType.Income)
			{
				account.Balance += transactionFormModel.Amount;
			}
			else if (transactionFormModel.TransactionType == TransactionType.Expense)
			{
				account.Balance -= transactionFormModel.Amount;
			}

			await data.Transactions.AddAsync(newTransaction);

			await data.SaveChangesAsync();
		}

		public async Task CreateAccount(string userId, CreateAccountFormModel accountModel)
		{
			var newAccount = new Data.Models.Account()
			{
				Name = accountModel.Name,
				Balance = accountModel.Balance,
				AccountTypeId = accountModel.AccountTypeId,
				CurrencyId = accountModel.CurrencyId,
				OwnerId = userId
			};

			await data.Accounts.AddAsync(newAccount);

			await data.SaveChangesAsync();

			if (newAccount.Balance != 0)
			{
				var newTransaction = new Data.Models.Transaction()
				{
					Amount = newAccount.Balance,
					AccountId = newAccount.Id,
					CategoryId = 1, // Id for initial balance 
					Refference = "Initial Balance"
				};

				await data.Transactions.AddAsync(newTransaction);
			}

			await data.SaveChangesAsync();
		}

		public async Task<IEnumerable<TransactionViewModel>> LastFiveTransactions(string userId)
		{
			return await data.Transactions
				.Where(t => t.Account.OwnerId == userId)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.Select(t => new TransactionViewModel
				{
					Account = t.Account.Name,
					Currency = t.Account.Currency.Name,
					Amount = t.Amount,
					TransactionType = t.TransactionType.ToString(),
					Category = t.Category.Name,
					CreatedOn = t.CreatedOn,
					Refference = t.Refference
				})
				.ToArrayAsync();
		}
	}
}
