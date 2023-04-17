namespace PersonalFinancer.Web.Infrastructure
{
	using AutoMapper;

	using Data.Models;

	using Services.Accounts.Models;
	using Services.Messages.Models;
	using Services.Shared.Models;
	using Services.User.Models;

	using Web.Models.Account;
	using Web.Models.Message;
	using Web.Models.Shared;
	using Web.Models.Transaction;
	using Web.Models.User;

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
			CreateMap<RegisterFormViewModel, ApplicationUser>()
				.ForMember(m => m.UserName, mf => mf.MapFrom(s => s.Email));

			CreateMap<MessageDetailsServiceModel, MessageDetailsViewModel>();
		}
	}
}
