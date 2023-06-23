namespace PersonalFinancer.Web
{
	using AutoMapper;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.Models.Account;
	using PersonalFinancer.Web.Models.Message;
	using PersonalFinancer.Web.Models.Transaction;

	public class ControllerMappingProfile : Profile
    {
        public ControllerMappingProfile()
        {
			this.CreateMap<AccountDetailsServiceModel, AccountDetailsViewModel>();
			this.CreateMap<AccountDetailsShortServiceModel, AccountDetailsViewModel>();
			this.CreateMap<AccountFormViewModel, AccountFormShortServiceModel>().ReverseMap();

			this.CreateMap<TransactionsServiceModel, UserTransactionsViewModel>();
			this.CreateMap<UserTransactionsInputModel, UserTransactionsViewModel>();
			this.CreateMap<UserAccountsAndCategoriesServiceModel, TransactionFormModel>();

			this.CreateMap<ApplicationUser, AccountFormViewModel>();

			this.CreateMap<MessageDetailsServiceModel, MessageDetailsViewModel>();
        }
    }
}
