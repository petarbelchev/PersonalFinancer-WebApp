namespace PersonalFinancer.Web
{
    using AutoMapper;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Services.Accounts.Models;
    using PersonalFinancer.Services.Messages.Models;
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Models.Account;
    using PersonalFinancer.Web.Models.Api;
	using PersonalFinancer.Web.Models.Home;
	using PersonalFinancer.Web.Models.Message;
	using PersonalFinancer.Web.Models.Shared;
	using PersonalFinancer.Web.Models.Transaction;

    public class ControllerMappingProfile : Profile
    {
        public ControllerMappingProfile()
        {
			this.CreateMap<AccountDetailsDTO, AccountDetailsViewModel>();
			this.CreateMap<AccountDetailsInputModel, AccountDetailsViewModel>();
			this.CreateMap<CreateEditAccountInputModel, CreateEditAccountViewModel>();
			this.CreateMap<CreateEditAccountOutputDTO, CreateEditAccountViewModel>();
			this.CreateMap<CreateEditAccountInputModel, CreateEditAccountInputDTO>();
			this.CreateMap<AccountTypesAndCurrenciesDropdownDTO, CreateEditAccountViewModel>();
			this.CreateMap<AccountTransactionsInputModel, AccountTransactionsFilterDTO>()
				.ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id));

			this.CreateMap<UserTransactionsApiInputModel, TransactionsFilterDTO>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

			this.CreateMap<DateFilterModel, UserDashboardViewModel>();
			this.CreateMap<UserDashboardDTO, UserDashboardViewModel>();

			this.CreateMap<CreateEditTransactionInputModel, CreateEditTransactionViewModel>();
			this.CreateMap<CreateEditTransactionInputModel, CreateEditTransactionInputDTO>();
			this.CreateMap<CreateEditTransactionOutputDTO, CreateEditTransactionViewModel>();
			this.CreateMap<AccountsAndCategoriesDropdownDTO, CreateEditTransactionViewModel>();

			this.CreateMap<Reply, ReplyOutputDTO>();

			this.CreateMap<Message, MessageDetailsDTO>();
			this.CreateMap<MessageDetailsDTO, MessageDetailsViewModel>();
        }
    }
}
