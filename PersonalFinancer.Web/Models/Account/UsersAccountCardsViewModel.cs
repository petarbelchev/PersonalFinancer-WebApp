namespace PersonalFinancer.Web.Models.Account
{
    using PersonalFinancer.Services.Shared.Models;
    using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Services.Constants;
    using static PersonalFinancer.Web.Constants;

    public class UsersAccountCardsViewModel
	{
		public IEnumerable<AccountCardServiceModel> Accounts { get; set; } = null!;

		public string ApiAccountsEndpoint { get; set; }
			= HostConstants.ApiAccountsUrl;

		public PaginationModel Pagination { get; set; } = new PaginationModel()
		{
			ElementsPerPage = PaginationConstants.AccountsPerPage,
			ElementsName = PaginationConstants.AccountsName
		};

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Area = "Admin",
			Controller = "Accounts",
			Action = "Index"
		};
	}
}
