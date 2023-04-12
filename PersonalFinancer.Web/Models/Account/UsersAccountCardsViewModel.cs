namespace PersonalFinancer.Web.Models.Account
{
    using Services.Shared.Models;

    using Web.Models.Shared;

    using static Data.Constants;

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
