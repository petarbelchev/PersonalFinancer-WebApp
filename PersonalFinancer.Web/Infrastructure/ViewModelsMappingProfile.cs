namespace PersonalFinancer.Web.Infrastructure
{
	using AutoMapper;

	using Data.Models;

	using Services.Accounts.Models;
	using Services.AccountTypes.Models;
	using Services.Categories.Models;
	using Services.Currencies.Models;
	using Services.Shared.Models;
	using Services.User.Models;

	using Web.Models.Accounts;
	using Web.Models.Currencies;
	using Web.Models.Home;
	using Web.Models.Shared;
	using Web.Models.User;
	using Web.Models.AccountTypes;

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
			CreateMap<RegisterFormModel, ApplicationUser>()
				.ForMember(m => m.UserName, mf => mf.MapFrom(s => s.Email));

			CreateMap<UserDetailsDTO, UserDetailsViewModel>();
		}
	}
}
