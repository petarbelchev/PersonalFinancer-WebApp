namespace PersonalFinancer.Web.Models.Accounts
{
	using Web.Models.Shared;

	using static Data.Constants.HostConstants;

	public class UsersAccountCardsViewModel
	{
		public IEnumerable<AccountCardExtendedViewModel> Accounts { get; set; } = null!;

		public string ApiAccountsEndpoint { get; set; }
			= ApiAccountsUrl;

		public PaginationModel Pagination { get; set; }
			= new PaginationModel()
			{
				ElementsPerPage = 12,
				ElementsName = "accounts"
			};

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Area = "Admin",
			Controller = "Accounts",
			Action = "Index"
		};
	}
}
