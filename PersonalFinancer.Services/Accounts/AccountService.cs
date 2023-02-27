namespace PersonalFinancer.Services.Accounts
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Microsoft.EntityFrameworkCore;

	using Models;
	using Infrastructure;
	using Category;
	using Transactions;
	using Transactions.Models;
	using Data;
	using Data.Enums;
	using Data.Models;
	using static Data.DataConstants.CategoryConstants;

	public class AccountService : IAccountService
	{
		private readonly PersonalFinancerDbContext data;
		private readonly IMapper mapper;
		private readonly ITransactionsService transactionsService;
		private readonly ICategoryService categoryService;

		public AccountService(
			PersonalFinancerDbContext context,
			IMapper mapper,
			ITransactionsService transactionsService,
			ICategoryService categoryService)
		{
			this.data = context;
			this.mapper = mapper;
			this.transactionsService = transactionsService;
			this.categoryService = categoryService;
		}

		/// <summary>
		/// Returns collection of User's accounts with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountDropdownViewModel>> AllAccountsDropdownViewModel(string userId)
		{
			IEnumerable<AccountDropdownViewModel> accounts = await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.ToArrayAsync();

			return accounts;
		}

		/// <summary>
		/// Returns Account with Id and Name or null.
		/// </summary>
		public async Task<AccountDropdownViewModel?> AccountDropdownViewModel(Guid accountId)
		{
			AccountDropdownViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId)
				.Select(a => mapper.Map<AccountDropdownViewModel>(a))
				.FirstOrDefaultAsync();

			return account;
		}

		/// <summary>
		/// Returns Account with Id, Name, Balance and all Transactions or null.
		/// </summary>
		public async Task<AccountDetailsViewModel?> AccountDetailsViewModel(Guid accountId)
		{
			AccountDetailsViewModel? account = await data.Accounts
				.Where(a => a.Id == accountId && !a.IsDeleted)
				.ProjectTo<AccountDetailsViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.FirstOrDefaultAsync();

			return account;
		}

		/// <summary>
		/// Returns collection of Account Types for the current user with Id and Name.
		/// </summary>
		public async Task<IEnumerable<AccountTypeViewModel>> AccountTypesViewModel(string userId)
		{
			return await data.AccountTypes
				.Where(a => (a.UserId == null || a.UserId == userId) && !a.IsDeleted)
				.Select(a => mapper.Map<AccountTypeViewModel>(a))
				.ToArrayAsync();
		}

		/// <summary>
		/// Creates a new Account and if the new account has initial balance creates new Transaction with given amount.
		/// Returns new Account's id.
		/// </summary>
		/// <param name="userId">User's identifier</param>
		/// <param name="accountModel">Model with Name, Balance, AccountTypeId, CurrencyId.</param>
		public async Task<Guid> CreateAccount(string userId, AccountFormModel accountModel)
		{
			Account newAccount = new Account()
			{
				Name = accountModel.Name,
				Balance = accountModel.Balance,
				AccountTypeId = accountModel.AccountTypeId,
				CurrencyId = accountModel.CurrencyId,
				OwnerId = userId
			};

			await data.Accounts.AddAsync(newAccount);

			if (newAccount.Balance != 0)
			{
				await transactionsService.CreateTransaction(new TransactionFormModel
				{
					Amount = newAccount.Balance,
					AccountId = newAccount.Id,
					CategoryId = await categoryService.CategoryIdByName(CategoryInitialBalanceName),
					Refference = "Initial Balance",
					CreatedOn = DateTime.UtcNow,
					TransactionType = TransactionType.Income
				}, true);
			}

			await data.SaveChangesAsync();

			return newAccount.Id;
		}

		/// <summary>
		/// Checks is the given User is owner of the given account, if does not exist, throws an exception.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<bool> IsAccountOwner(string userId, Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.OwnerId == userId;
		}

		/// <summary>
		/// Checks is the given Account deleted, if does not exist, throws an exception.
		/// </summary>
		/// <param name="accountId"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<bool> IsAccountDeleted(Guid accountId)
		{
			Account? account = await data.Accounts.FindAsync(accountId);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			return account.IsDeleted;
		}

		/// <summary>
		/// Delete an Account and give the option to delete all of the account's transactions.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task DeleteAccountById(Guid accountId, bool shouldDeleteTransactions)
		{
			Account? account = await data.Accounts
				.FirstOrDefaultAsync(a => a.Id == accountId && !a.IsDeleted);

			if (account == null)
			{
				throw new ArgumentNullException("Account does not exist.");
			}

			if (shouldDeleteTransactions)
			{
				data.Accounts.Remove(account);
			}
			else
			{
				account.IsDeleted = true;
			}

			await data.SaveChangesAsync();
		}

		/// <summary>
		/// Returns Dashboard View Model for current User with Last transactions, Accounts and Currencies Cash Flow.
		/// Throws Exception when End Date is before Start Date.
		/// </summary>
		/// <param name="model">Model with Start and End Date which are selected period of transactions.</param>
		/// <exception cref="ArgumentException"></exception>
		public async Task DashboardViewModel(string userId, DashboardServiceModel model)
		{
			model.Accounts = await data.Accounts
				.Where(a => a.OwnerId == userId && !a.IsDeleted)
				.ProjectTo<AccountCardViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.ToArrayAsync();

			if (model.StartDate > model.EndDate)
			{
				throw new ArgumentException("Start Date must be before End Date.");
			}

			model.LastTransactions = await data.Transactions
				.Where(t =>
					t.Account.OwnerId == userId &&
					t.CreatedOn >= model.StartDate &&
					t.CreatedOn <= model.EndDate)
				.OrderByDescending(t => t.CreatedOn)
				.Take(5)
				.ProjectTo<TransactionShortViewModel>(new MapperConfiguration(
					cfg => cfg.AddProfile<ServiceMappingProfile>()))
				.ToArrayAsync();

			await data.Accounts
				.Where(a => a.OwnerId == userId && a.Transactions.Any())
				.Include(a => a.Currency)
				.Include(a => a.Transactions
					.Where(t => t.CreatedOn >= model.StartDate && t.CreatedOn <= model.EndDate))
				.ForEachAsync(a =>
				{
					if (!model.CurrenciesCashFlow.ContainsKey(a.Currency.Name))
					{
						model.CurrenciesCashFlow[a.Currency.Name] = new CashFlowViewModel();
					}

					decimal? income = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Income)
						.Sum(t => t.Amount);

					if (income != null)
					{
						model.CurrenciesCashFlow[a.Currency.Name].Income += (decimal)income;
					}

					decimal? expense = a.Transactions?
						.Where(t => t.TransactionType == TransactionType.Expense)
						.Sum(t => t.Amount);

					if (expense != null)
					{
						model.CurrenciesCashFlow[a.Currency.Name].Expence += (decimal)expense;
					}
				});
		}
	}
}