using AutoMapper;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.Messages.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;
using PersonalFinancer.Web.Models.Account;
using PersonalFinancer.Web.Models.Message;
using PersonalFinancer.Web.Models.Shared;
using PersonalFinancer.Web.Models.Transaction;

namespace PersonalFinancer.Web.Infrastructure
{
	public class ControllerMappingProfile : Profile
	{
		public ControllerMappingProfile()
		{
			CreateMap<Account, DeleteAccountViewModel>();
			CreateMap<Account, AccountFormViewModel>();
			CreateMap<AccountDetailsServiceModel, AccountDetailsViewModel>();
			CreateMap<AccountFormServiceModel, AccountFormViewModel>();
			CreateMap<AccountFormViewModel, AccountFormShortServiceModel>();
			CreateMap<AccountDetailsShortServiceModel, AccountDetailsViewModel>();

			CreateMap<Transaction, TransactionFormModel>();
			CreateMap<TransactionFormModel, TransactionFormShortServiceModel>();
			CreateMap<TransactionFormServiceModel, TransactionFormModel>();
			CreateMap<TransactionsServiceModel, UserTransactionsViewModel>();
			CreateMap<DateFilterModel, UserTransactionsViewModel>();
			CreateMap<UserAccountsAndCategoriesServiceModel, TransactionFormModel>();

			CreateMap<ApplicationUser, AccountFormViewModel>();

			CreateMap<MessageDetailsServiceModel, MessageDetailsViewModel>();
		}
	}
}
