using AutoMapper;

using PersonalFinancer.Data.Models;

using PersonalFinancer.Services.Accounts.Models;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Categories.Models;
using PersonalFinancer.Services.Currencies.Models;
using PersonalFinancer.Services.Shared.Models;
using PersonalFinancer.Services.User.Models;

using PersonalFinancer.Web.Models.Accounts;
using PersonalFinancer.Web.Models.Currencies;
using PersonalFinancer.Web.Models.Home;
using PersonalFinancer.Web.Models.Shared;
using PersonalFinancer.Web.Models.User;
using PersonalFinancer.Web.Models.AccountTypes;

namespace PersonalFinancer.Web.Infrastructure
{
    public class ViewModelsMappingProfile : Profile
	{
		public ViewModelsMappingProfile()
		{
			CreateMap<CategoryOutputDTO, CategoryViewModel>();
			CreateMap<CategoryInputModel, CategoryInputDTO>();

			CreateMap<CurrencyInputModel, CurrencyInputDTO>();
			CreateMap<CurrencyOutputDTO, CurrencyViewModel>();
			CreateMap<CurrencyCashFlowDTO, CurrencyCashFlowViewModel>();

			CreateMap<AccountCardExtendedDTO, AccountCardExtendedViewModel>();
			CreateMap<AccountDTO, AccountCardExtendedViewModel>(); 
			CreateMap<AccountDTO, AccountDropdownViewModel>(); 
			CreateMap<AccountCardDTO, AccountCardViewModel>(); 
			CreateMap<DeleteAccountDTO, DeleteAccountViewModel>(); 
			CreateMap<CreateAccountFormDTO, CreateAccountFormModel>().ReverseMap(); 
			CreateMap<EditAccountFormDTO, EditAccountFormModel>().ReverseMap(); 

			CreateMap<AccountTypeInputModel, AccountTypeInputDTO>();
			CreateMap<AccountTypeOutputDTO, AccountTypeViewModel>();

			CreateMap<UserDashboardDTO, UserDashboardViewModel>();

			CreateMap<TransactionDetailsDTO, TransactionDetailsViewModel>();
			CreateMap<EmptyTransactionFormDTO, TransactionFormModel>();
			CreateMap<TransactionDetailsDTO, TransactionDetailsViewModel>();
			CreateMap<TransactionFormModel, FulfilledTransactionFormDTO>().ReverseMap();
			CreateMap<TransactionFormModel, CreateTransactionInputDTO>();
			CreateMap<TransactionTableDTO, TransactionTableViewModel>();

			CreateMap<AccountDetailsOutputDTO, AccountDetailsViewModel>();

			CreateMap<AccountTransactionsInputModel, AccountTransactionsInputDTO>();

			CreateMap<UserTransactionsApiInputModel, UserTransactionsApiInputDTO>();
			CreateMap<UserTransactionsOutputDTO, UserTransactionsViewModel>();
			
			CreateMap<UserDTO, UserViewModel>();
			CreateMap<ApplicationUser, CreateAccountFormModel>();
			
			CreateMap<UserDetailsDTO, UserDetailsViewModel>();
		}
	}
}
