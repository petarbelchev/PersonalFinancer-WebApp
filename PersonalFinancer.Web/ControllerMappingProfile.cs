namespace PersonalFinancer.Web
{
	using AutoMapper;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.Models.Account;
	using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.Message;
	using PersonalFinancer.Web.Models.Transaction;

	public class ControllerMappingProfile : Profile
    {
        public ControllerMappingProfile()
        {
			this.CreateMap<AccountDetailsLongDTO, AccountDetailsViewModel>();
			this.CreateMap<AccountDetailsShortDTO, AccountDetailsViewModel>();
			this.CreateMap<AccountFormViewModel, CreateEditAccountDTO>().ReverseMap();
			this.CreateMap<AccountTypesAndCurrenciesDropdownDTO, AccountFormViewModel>();

			this.CreateMap<TransactionsDTO, UserTransactionsViewModel>();
			this.CreateMap<TransactionsFilterDTO, UserTransactionsViewModel>();
			this.CreateMap<UserTransactionsInputModel, UserTransactionsViewModel>();
			this.CreateMap<TransactionsPageDTO, UserTransactionsViewModel>();
			this.CreateMap<UserTransactionsInputModel, TransactionsFilterDTO>();

			this.CreateMap<UserTransactionsApiInputModel, TransactionsFilterDTO>()
				.ForMember(m => m.UserId, mf => mf.MapFrom(s => s.Id));

			this.CreateMap<TransactionFormViewModel, CreateEditTransactionDTO>().ReverseMap();
			this.CreateMap<AccountsAndCategoriesDropdownDTO, TransactionFormViewModel>();

			this.CreateMap<ApplicationUser, AccountFormViewModel>();

			this.CreateMap<MessageDetailsDTO, MessageDetailsViewModel>();
        }
    }
}
