namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Services.Constants;
    using static PersonalFinancer.Web.Constants;

    public class UsersAccountsCardsViewModel
	{
        public UsersAccountsCardsViewModel(AccountsCardsDTO usersCardsData)
        {
			this.AccountsCards = usersCardsData.Accounts;

			this.ApiAccountsEndpoint = UrlPathConstants.ApiAccountsEndpoint;

			this.Pagination = new PaginationModel(
				PaginationConstants.AccountsName, 
				PaginationConstants.AccountsPerPage,
				usersCardsData.TotalAccountsCount);

			this.Routing = new RoutingModel
			{
				Area = "Admin",
				Controller = "Accounts",
				Action = "Index"
			};
		}

        public IEnumerable<AccountCardDTO> AccountsCards { get; set; }

		public string ApiAccountsEndpoint { get; set; }

		public PaginationModel Pagination { get; private set; }

		public RoutingModel Routing { get; set; }
	}
}
