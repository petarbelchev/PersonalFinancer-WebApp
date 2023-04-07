using PersonalFinancer.Web.Models.Shared;
using static PersonalFinancer.Data.Constants.HostConstants;

namespace PersonalFinancer.Web.Models.Accounts
{
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
