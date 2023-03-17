using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AllUsersAccountCardsViewModel
	{
		public IEnumerable<AccountCardExtendedViewModel> Accounts { get; set; } = null!;

		public PaginationModel Pagination { get; set; }
			= new PaginationModel()
			{
				ElementsPerPage = 12,
				ElementsName = "accounts"
			};

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Area = "Admin",
			Controller = "Account",
			Action = "Index"
		};
	}
}
