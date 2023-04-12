namespace PersonalFinancer.Web.Infrastructure
{
    using AutoMapper;

    using Data.Models;
    using PersonalFinancer.Web.Models.Transaction;
    using Web.Models.Account;

    public class ControllerMappingProfile : Profile
	{
		public ControllerMappingProfile()
		{

			CreateMap<Account, DeleteAccountViewModel>();
			CreateMap<Account, AccountFormViewModel>();
			CreateMap<Transaction, TransactionFormModel>();
			CreateMap<ApplicationUser, AccountFormViewModel>();
		}
	}
}
